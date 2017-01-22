(ns game.player
  (:use arcadia.core
        arcadia.linear)
  (:import [UnityEngine Input Physics Rigidbody]))

(defn punch-nazis [player]
  (when (Input/GetKeyDown "space")
    (let [colliders (Physics/OverlapSphere
                      (.. player transform position)
                      10)
          rbs (map #(.GetComponent % Rigidbody) colliders)
          rbs-we-want (->> rbs
                           (map obj-nil)
                           (remove nil?))]
      (doseq [rb rbs-we-want]
        (.AddForce rb (v3* (.. player transform forward) 10000))))))


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
  (hook+ Selection/activeObject :update #'punch-nazis)
  
  
  
  (make-cubes)
  )
