(ns game.effects
  (:use arcadia.core
        arcadia.linear)
  (:require [game.ragdoll :as ragdoll]
            [game.replay :as replay])
  (:import [UnityEngine Mathf Camera Animator Input Physics Rigidbody Vector3]))

(defn sticky [go other]
  (when (= "Hips_jnt" (.name other))
    (replay/register-moment (.. other transform position))
    (let [other-root (.. other transform root)
          other-rb (.GetComponent other Rigidbody)
          other-rbs (.GetComponentsInChildren other-root Rigidbody)]
      (doseq [rb other-rbs]
        (set! (.velocity rb) (v3 0)))
      (set! (.isKinematic other-rb) true))))

(defn amplify-collision [go collision]
  (when-let [this-anim (.GetComponentInParent go Animator)]
    (when-let [other-anim (.. collision gameObject (GetComponentInParent Animator))]
      (when-not (.enabled other-anim)
        (set! (.enabled this-anim) false)))))

(comment
  (doseq [go (->> (objects-tagged "Record Me")
                  (mapcat #(.GetComponentsInChildren % Rigidbody))
                  (map #(.gameObject %)))]
    (hook+ go :on-collision-enter #'amplify-collision))
  
  (doseq [win Selection/objects]
    (hook+ win :on-trigger-enter #'sticky)
    )
  )