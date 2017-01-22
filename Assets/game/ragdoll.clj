(ns game.ragdoll
  (:use arcadia.core
        arcadia.linear)
  (:import [UnityEngine Rigidbody Physics
            Collider BoxCollider CapsuleCollider SphereCollider
            Joint HingeJoint SpringJoint]))

(defn right-side? [obj]
  (boolean (re-find #"_R$" (.name obj))))

(defn invert-collider [obj]
  (when (right-side? obj)
    (when-let [collider (cmpt obj Collider)]
      (set! (.center collider)
            (v3* (.center collider) -1)))))

(defmulti ragdoll*
  (fn [obj] (keyword (re-find #"^[^_]+" (.name obj)))))

(defn ragdoll! [obj]
  (ragdoll* obj)
  (invert-collider obj)
  (doseq [child (children obj)]
    (ragdoll! child)))

(defmethod ragdoll* :default [obj]
  (log "Skipping " (.name obj)))

(defmethod ragdoll* :Root [obj]
  (let [cc (cmpt+ obj BoxCollider)
        rb (cmpt+ obj Rigidbody)]
    (set! (.radius cc) (float 0.07))
    (set! (.direction cc) (int 0))
    (set! (.height cc) (float 0.4))
    (set! (.center cc) (v3 0.2 0 0))))

(defmethod ragdoll* :Hip [obj]
  (let [cc (cmpt+ obj CapsuleCollider)
        rb (cmpt+ obj Rigidbody)]
    (set! (.radius cc) (float 0.07))
    (set! (.direction cc) (int 0))
    (set! (.height cc) (float 0.4))
    (set! (.center cc) (v3 0.2 0 0))))

(defmethod ragdoll* :Knee [obj]
  (let [cc (cmpt+ obj CapsuleCollider)
        rb (cmpt+ obj Rigidbody)]
    (when-let [parent-rb (cmpt (parent obj) Rigidbody)]
      (let [hj (cmpt+ obj HingeJoint)]
        (set! (.connectedBody hj) parent-rb)))
    (set! (.radius cc) (float 0.07))
    (set! (.direction cc) (int 0))
    (set! (.height cc) (float 0.5))
    (set! (.center cc) (v3 0.15 0 0))))

(defmethod ragdoll* :Ankle [obj]
  (let [cc (cmpt+ obj CapsuleCollider)
        rb (cmpt+ obj Rigidbody)]
    (when-let [parent-rb (cmpt (parent obj) Rigidbody)]
      (let [hj (cmpt+ obj HingeJoint)]
        (set! (.connectedBody hj) parent-rb)))
    (set! (.radius cc) (float 0.05))
    (set! (.direction cc) (int 1))
    (set! (.height cc) (float 0.38))
    (set! (.center cc) (v3 0.05 -0.1 0))))

(defmethod ragdoll* :Shoulder [obj]
  (let [cc (cmpt+ obj CapsuleCollider)
        rb (cmpt+ obj Rigidbody)]
    (when-let [parent-rb (cmpt (parent obj) Rigidbody)]
      (let [hj (cmpt+ obj HingeJoint)]
        (set! (.connectedBody hj) parent-rb)))
    (set! (.radius cc) (float 0.05))
    (set! (.direction cc) (int 0))
    (set! (.height cc) (float 0.4))
    (set! (.center cc) (v3 0.12 0 0))))

(defmethod ragdoll* :Elbow [obj]
  (let [cc (cmpt+ obj CapsuleCollider)
        rb (cmpt+ obj Rigidbody)]
    (when-let [parent-rb (cmpt (parent obj) Rigidbody)]
      (let [hj (cmpt+ obj HingeJoint)]
        (set! (.connectedBody hj) parent-rb)))
    (set! (.radius cc) (float 0.05))
    (set! (.direction cc) (int 0))
    (set! (.height cc) (float 0.3))
    (set! (.center cc) (v3 0.12 0 0))))

(defmethod ragdoll* :Wrist [obj]
  (let [cc (cmpt+ obj BoxCollider)
        rb (cmpt+ obj Rigidbody)]
    (when-let [parent-rb (cmpt (parent obj) Rigidbody)]
      (let [hj (cmpt+ obj HingeJoint)]
        (set! (.connectedBody hj) parent-rb)))
    (set! (.size cc) (v3 0.15 0.1 0.05))
    (set! (.center cc) (v3 0.1 0 0))))

(defmethod ragdoll* :Head [obj]
  (let [cc (cmpt+ obj SphereCollider)
        rb (cmpt+ obj Rigidbody)]
    (when-let [parent-rb (cmpt (parent obj) Rigidbody)]
      (let [hj (cmpt+ obj HingeJoint)]
        (set! (.connectedBody hj) parent-rb)))
    (set! (.radius cc) (float 0.11))
    (set! (.center cc) (v3 -0.1 0 0))))

(defmethod ragdoll* :Chest [obj]
  (let [cc (cmpt+ obj BoxCollider)
        rb (cmpt+ obj Rigidbody)]
    (when-let [parent-rb (cmpt (parent obj) Rigidbody)]
      (let [hj (cmpt+ obj HingeJoint)]
        (set! (.connectedBody hj) parent-rb)))
    (set! (.size cc) (v3 0.5 0.25 0.35))
    ; (set! (.center cc) (v3 0.1 0 0))
    ))