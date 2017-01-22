(ns stuff
  (:use arcadia.core
        arcadia.linear)
  (:import [UnityEngine Rigidbody Physics
            Collider BoxCollider CapsuleCollider SphereCollider
            Joint HingeJoint SpringJoint]))



(comment 
  (require 'stuff :reload)
  
  (.AddForce (cmpt Selection/activeObject Rigidbody) (v3 10000 0 0))
  
  (do 
    ; (UnityEngine.Application/LoadLevel (UnityEngine.Application/loadedLevel))
    (doseq [col (.GetComponentsInChildren (object-named "Root_M") Collider)]
      (GameObject/DestroyImmediate col))
    (doseq [col (.GetComponentsInChildren (object-named "Root_M") Joint)]
      (GameObject/DestroyImmediate col))
    (doseq [col (.GetComponentsInChildren (object-named "Root_M") Rigidbody)]
      (GameObject/DestroyImmediate col))
      
    (ragdoll! (object-named "Root_M")))
  )