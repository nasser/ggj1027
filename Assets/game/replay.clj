(ns game.replay
  (:use arcadia.core
        arcadia.linear)
  (:import Snapshots Snapshot PlayerController
           [UnityStandardAssets.Characters.FirstPerson FirstPersonController]
           [UnityEngine CharacterController
            Application GUILayer GUIText Animator AudioListener GameObject
            Input Physics Camera Rigidbody Collider Time Joint
            Vector3 Quaternion]))

(def moments-of-interest (atom {}))

(defn register-moment [^Vector3 position]
  (swap! moments-of-interest update Time/frameCount conj position))

(defn reset-moments []
  (reset! moments-of-interest {}))

(def recording (Snapshots.))

(defn snapshot [^GameObject obj ^Snapshots record initial-frame]
  (.Push record
         Time/frameCount
         obj
         (.. obj transform localPosition)
         (.. obj transform localRotation)))

(defn start-recording [obj record]
  (when (cmpt obj Rigidbody)
    (let [initial-frame Time/frameCount]
      (hook+ obj :late-update ::replay #(snapshot % record initial-frame))))
  (doseq [child (children obj)]
    (start-recording child record)))

(defn start-recording-all [record]
  (doseq [r (objects-tagged "Record Me")]
    (start-recording r record)))

(defn stop-recording [obj]
  (hook- obj :late-update ::replay)
  (doseq [child (children obj)]
    (stop-recording child)))

(defn stop-recording-all []
  (doseq [r (objects-tagged "Record Me")]
    (stop-recording r)))

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
  (when (.. obj (GetComponentInParent Animator))
    (destroy (.. obj (GetComponentInParent Animator))))
  (when (.. obj (GetComponent Animator))
    (destroy (.. obj (GetComponent Animator))))
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

(defn cool-backwards-pan [go]
  (.. go transform
      (Translate (v3* (.. go transform right) (* Time/deltaTime -2)))))

(defn playback! [record moi speed]
  (let [playback-camera (GameObject. "playback camera")
        birds-eye (v3 -203.02 16.81 -210.64)
        playback-object (GameObject. (str (gensym "playback")))
        frames (vals record)
        first-frame (apply min (keys record))
        last-frame (apply max (keys record))
        f (volatile! (apply min (keys record)))
        gui (cmpt (object-named "Percent GUI") GUIText)]
    ;; (.SetActive (object-named "FPSController") false)
    (set! (.. (object-named "FPSController") (GetComponent FirstPersonController) enabled) false)
    (set! (.. (object-named "FPSController") (GetComponent PlayerController) enabled) false)
    (set! (.. (object-named "FPSController") (GetComponent CharacterController) enabled) false)
    (set! (.. (object-named "FirstPersonCharacter") (GetComponent Camera) enabled) false)
    (set! (.. (cmpt (object-named "Timer GUI") GUIText) enabled) false)
    (set! (.. playback-camera transform position) birds-eye)
    (.. playback-camera transform (LookAt (.transform (object-named "road_Roundabout"))))
    (set! (.. (object-named "Press Space GUI") (GetComponent GUIText) enabled) true)
    (set! (.text gui)
          (str (int (* 100 (/ (count (remove #(.enabled %) (objects-typed Animator)))
                              (count (objects-typed Animator))))) "%"))
    (cmpt+ playback-camera Camera)
    ; (cmpt+ playback-camera AudioListener)
    (cmpt+ playback-camera GUILayer)
    (doseq [frame frames]
      (doseq [object (keys frame)]
        (disable-physics object)))
    (hook+ playback-object
           :update
           (fn [go]
             (if (> (int @f) last-frame)
               (destroy playback-object)
               (seek record @f))
             (vswap! f + speed)))
    (hook+ playback-camera
           :update
           (fn [go]
             (cool-backwards-pan go)
             (when (Input/GetKeyDown "space")
               (require 'game.replay :reload)
               (require 'game.player :reload)
               (Application/LoadLevel "Title"))
             (when (< (.fontSize gui) 500)
               (set! (.fontSize gui)
                     (int (inc (.fontSize gui)))))))))

(comment
  (require 'game.replay :reload)
  (arcadia.compiler/aot-namespace "." 'arcadia.literals)
  arcadia.literals/parse-$ArrayType=4
  (start-recording-all recording)
  (stop-recording-all)
  (def recording* (digest recording))
  
  (count recording*)
    
  (time (take 3 (group-by #(.time %) (.snapshots recording))))
  
  (count (.snapshots recording))
  (seek% recording* 0.2)
  (playback! recording* 0.5)
  
  (doseq [cube (objects-named #".*Cube.*")]
    (start-recording cube recording)
    ;; (stop-recording cube)
    )
  )