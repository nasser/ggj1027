(ns game.replay
  (:use arcadia.core
        arcadia.linear)
  (:import [UnityEngine GameObject Input Physics Rigidbody Collider Time]))

;; { 90 { #obj { :position [1 2 3] :rotation  } }

(def recording (agent {}))

(defn snapshot [^GameObject obj record]
  (send record
        update-in
        [Time/frameCount obj]
        assoc
        :position (.. obj transform localPosition)
        :rotation (.. obj transform localRotation)
        :scale (.. obj transform localScale)))

(defn start-recording [obj record]
  (hook+ obj :late-update ::replay #(snapshot % record))
  (doseq [child (children obj)]
    (start-recording child record)))

(defn stop-recording [obj]
  (hook- obj :late-update ::replay)
  (doseq [child (children obj)]
    (stop-recording child)))

(defn playback! [record]
  (let [record @record
        sym (gensym "playback")
        playback-object (GameObject. (str sym))
        frames (vals record)
        f (volatile! (apply min (keys record)))
        last-frame (apply max (keys record))]
    ;; disable physics on target objects
    (doseq [frame frames]
      (doseq [object (keys frame)]
        (cmpt- object Rigidbody)
        (cmpt- object Collider)))
    (hook+ playback-object
           :update
           sym
           (fn [go]
             (if (> (int @f) last-frame)
               (hook- playback-object :update sym)
               (doseq [[^GameObject
                        obj {:keys [position rotation scale]}]
                       (record (int @f))]
                 (set! (.. obj transform localPosition) position)
                 (set! (.. obj transform localRotation) rotation)
                 (set! (.. obj transform localScale) scale)))
             (vswap! f + playback-speed)))))

(comment
  (playback! recording)
  (doseq [cube (objects-named #".*Cube.*")]
    (stop-recording cube)
    )
  )