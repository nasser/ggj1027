(ns game.replay
  (:use arcadia.core
        arcadia.linear)
  (:import Snapshots Snapshot
           [UnityEngine GameObject Input Physics 
            Rigidbody Collider Time Joint
            Vector3 Quaternion]))

(def recording (Snapshots.))

(defn snapshot [^GameObject obj ^Snapshots record initial-frame]
  (.Push record
         (- Time/frameCount initial-frame)
         obj
         (.. obj transform localPosition)
         (.. obj transform localRotation)))

(defn start-recording [obj record]
  (when (cmpt obj Rigidbody)
    (let [initial-frame Time/frameCount]
      (hook+ obj :late-update ::replay #(snapshot % record initial-frame))))
  (doseq [child (children obj)]
    (start-recording child record)))

(defn stop-recording [obj]
  (hook- obj :late-update ::replay)
  (doseq [child (children obj)]
    (stop-recording child)))

(defn digest [record]
  (persistent!
    (reduce (fn [record* ^Snapshot snap]
              (assoc! record*
                      (.time snap)
                      (assoc (record* (.time snap)) (.gameObject snap) snap)))
            (transient {})
            (.snapshots record))))

;; TODO disable animators
(defn disable-physics [obj]
  (cmpt- obj Joint)
  (cmpt- obj Rigidbody)
  (cmpt- obj Collider))

(defn seek [record frame]
  (let [lframe (long frame)
        prev-frame (record lframe)
        next-frame (record (inc lframe))
        pct (- frame lframe)]
    (if (and prev-frame next-frame)
      (dorun
        (map
          (fn [[^GameObject obj ^Snapshot prev-snap]
               [^GameObject obj* ^Snapshot next-snap]]
            (set! (.. obj transform localPosition)
                  (Vector3/Lerp (.position prev-snap)
                                (.position next-snap)
                                pct))
            (set! (.. obj transform localRotation)
                  (Quaternion/Slerp (.rotation prev-snap)
                                    (.rotation next-snap)
                                    pct)))
          prev-frame
          next-frame)))))

(defn seek% [record pct]
  (seek record (int (* pct (count record)))))

(defn playback! [record speed]
  (let [playback-object (GameObject. (str (gensym "playback")))
        frames (vals record)
        f (volatile! (apply min (keys record)))
        last-frame (apply max (keys record))]
    (doseq [frame frames]
      (doseq [object (keys frame)]
        (disable-physics object)))
    (hook+ playback-object
           :update
           (fn [go]
             (if (> (int @f) last-frame)
               (destroy playback-object)
               (seek record @f))
             (vswap! f + speed)))))

(comment
  (require 'game.replay :reload)
  (doseq [a (objects-typed UnityEngine.Animator)]
    (start-recording a recording))
  (doseq [a (objects-typed UnityEngine.Animator)]
    (stop-recording a))
  
  (def recording* (digest recording))
  
  (take 1 recording*)
  
  (def recording* (digest recording))
  
  (time (take 3 (group-by #(.time %) (.snapshots recording))))
  
  (count (.snapshots recording))
  (seek% recording* 0.2)
  (playback! recording* 0.01)
  
  (doseq [cube (objects-named #".*Cube.*")]
    (start-recording cube recording)
    ;; (stop-recording cube)
    )
  )