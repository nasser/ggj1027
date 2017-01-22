(ns game.player
  (:use arcadia.core
        arcadia.linear)
  (:require [game.ragdoll :as ragdoll]
            [game.replay :as replay])
  (:import [UnityEngine Time AudioSource Mathf Camera Animator Input Physics Rigidbody Vector3 GUIText]))

(defn windup-sound [obj]
  (let [as (.. obj (GetComponent AudioSource))]
    (when (and (Input/GetKey "f")
               (not (.isPlaying as)))
      (.Play as))
    (when-not (Input/GetKey "f")
      (.Stop as))))

(defn windup-effect [wind-up]
  (if wind-up
    (set! (.fov Camera/main)
          (float (+ (.fov Camera/main) 0.5)))
    (if (> (.fov Camera/main) 60.0)
      (set! (.fov Camera/main)
            (Mathf/Lerp (.fov Camera/main)
                        60.0
                        0.5)))))

(def player-state (atom {:started-timer false
                         :timer 15}))

(defn render-gui [player]
  (when (:started-timer @player-state)
    (set! (.text (cmpt (object-named "GUI") GUIText))
          (str (int (:timer @player-state)) "s\n"
               (int (* 100 (/ (count (remove #(.enabled %) (objects-typed Animator)))
                              (count (objects-typed Animator))))) "%"))
    (swap! player-state update :timer - Time/deltaTime)
    (when (<= (:timer @player-state) 0)
      (swap! player-state assoc :started-timer false)
      (replay/stop-recording-all)
      (replay/playback!
        (replay/digest replay/recording)
        @replay/moments-of-interest
        0.1))))

(defn punch-nazis [player]
  (windup-effect (Input/GetKey "f"))
  (when (Input/GetKeyUp "f")
    (swap! player-state assoc :started-timer true)
    (replay/start-recording-all replay/recording)
    (let [colliders (Physics/OverlapSphere
                      (.. player transform position)
                      10)
          rbs (map #(.GetComponent % Rigidbody) colliders)
          rbs-we-want (->> rbs
                           (map obj-nil)
                           (remove nil?))]
      (doseq [rb rbs-we-want]
        (when-let [anim (.GetComponentInParent rb Animator)]
          (set! (.enabled anim) false))
        (.AddForce rb (v3* (.. Camera/main transform forward) 4000))
        (.AddForce rb (v3* Vector3/up 1000))
        (.AddTorque rb (v3* (v3 (rand) (rand) (rand)) 4000))
        ))))

(defn make-cubes []
  (doseq [obj (objects-named #".*Cube.*")]
    (GameObject/DestroyImmediate obj))
  (let [cube (create-primitive :cube)]
    (set! (.. cube transform localScale) (v3 1 3 1))
    (for [x (range -20 20)
          y (range -20 20)]
      (let [new-object 
            (instantiate
              cube
              (v3+
                (v3 (* x 2) (* y 2) 0)
                (v3 (rand) (rand) (rand))))]
        (cmpt+ new-object Rigidbody)))))

(comment
  (require 'game.player :reload)
  (hook+ Selection/activeObject :update #'windup-sound)
  (hook+ Selection/activeObject :update #'punch-nazis)
  (hook+ Selection/activeObject :update #'render-gui)
  (make-cubes)
  
  ;; falling cubes
  (->> (objects-named #".*Cube.*")
       ; (filter #(when-let [rb (cmpt % Rigidbody)]
       ;            (> (.. rb velocity magnitude) 1)))
       (map destroy)
       dorun)
  
  
  )
