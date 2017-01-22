(ns game.player
  (:use arcadia.core
        arcadia.linear)
  (:require [game.ragdoll :as ragdoll]
            [game.replay :as replay])
  (:import [UnityEngine Mathf Camera Animator Input Physics Rigidbody Vector3]))

(defn windup-effect [wind-up]
  (if wind-up
    (set! (.fov Camera/main)
          (float (+ (.fov Camera/main) 0.5)))
    (if (> (.fov Camera/main) 60.0)
      (set! (.fov Camera/main)
            (Mathf/Lerp (.fov Camera/main)
                        60.0
                        0.5)))))

(defn punch-nazis [player]
  (windup-effect (Input/GetKey "return"))
  (when (Input/GetKeyUp "return")
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
  (hook+ Selection/activeObject :update #'punch-nazis)
  (make-cubes)
  
  ;; falling cubes
  (->> (objects-named #".*Cube.*")
       ; (filter #(when-let [rb (cmpt % Rigidbody)]
       ;            (> (.. rb velocity magnitude) 1)))
       (map destroy)
       dorun)
  
  
  )
