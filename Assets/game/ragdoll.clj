(ns game.ragdoll
  (:use arcadia.core
        arcadia.linear)
  (:import [UnityEngine Rigidbody Physics
            Collider BoxCollider CapsuleCollider SphereCollider
            Joint HingeJoint SpringJoint]
           RagdollBuilder))

(defn ragdoll!
  [{:keys [head middle-spine pelvis left-hips left-knee left-foot
           right-hips right-knee right-elbow right-arm left-elbow
           left-arm right-foot anchor
           total-mass flip-forward]}]
  (let [builder (RagdollBuilder.)]
    (when flip-forward
      (set! (.flipForward builder) flip-forward))
    (when total-mass
      (set! (.totalMass builder) total-mass))
    (set! (.head builder) (.transform head))
    (set! (.middleSpine builder) (.transform middle-spine))
    (set! (.pelvis builder) (.transform pelvis))
    (set! (.leftHips builder) (.transform left-hips))
    (set! (.leftKnee builder) (.transform left-knee))
    (set! (.leftFoot builder) (.transform left-foot))
    (set! (.rightHips builder) (.transform right-hips))
    (set! (.rightKnee builder) (.transform right-knee))
    (set! (.rightElbow builder) (.transform right-elbow))
    (set! (.rightArm builder) (.transform right-arm))
    (set! (.leftElbow builder) (.transform left-elbow))
    (set! (.leftArm builder) (.transform left-arm))
    (set! (.rightFoot builder) (.transform right-foot))
    (.CheckConsistency builder)
    (.OnWizardCreate builder)))

(defn path-to [obj]
  (->> obj
       (iterate parent)
       (take-while some?)
       reverse
       (map #(.name %))
       (clojure.string/join "/")))

(defn ragmap [obj hm]
  (reduce-kv
    (fn [m k v]
      (assoc m k (.. obj transform (Find v))))
    {}
    hm))

(def business
  {:left-foot "Group/Main/DeformationSystem/Root_M/Hip_L/Knee_L/Ankle_L"
   :left-hips "Group/Main/DeformationSystem/Root_M/Hip_L"
   :right-hips "Group/Main/DeformationSystem/Root_M/Hip_R"
   :right-foot "Group/Main/DeformationSystem/Root_M/Hip_R/Knee_R/Ankle_R"
   :right-arm "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Scapula_R/Shoulder_R"
   :right-elbow "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R"
   :head "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Neck_M/Head_M"
   :right-knee "Group/Main/DeformationSystem/Root_M/Hip_R/Knee_R"
   :left-knee "Group/Main/DeformationSystem/Root_M/Hip_L/Knee_L"
   :left-elbow "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Scapula_L/Shoulder_L/Elbow_L"
   :middle-spine "Group/Main/DeformationSystem/Root_M/Spine1_M"
   :left-arm "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Scapula_L/Shoulder_L"
   :pelvis "Group/Main/DeformationSystem/Root_M"})


(def simple
  {:left-foot "Hips_jnt/UpperLeg_Left_jnt/LowerLeg_Left_jnt/Foot_Left_jnt"
   :left-knee "Hips_jnt/UpperLeg_Left_jnt/LowerLeg_Left_jnt"
   :left-hips "Hips_jnt/UpperLeg_Left_jnt"
   :right-foot "Hips_jnt/UpperLeg_Right_jnt/LowerLeg_Right_jnt/Foot_Right_jnt"
   :right-knee "Hips_jnt/UpperLeg_Right_jnt/LowerLeg_Right_jnt"
   :right-hips "Hips_jnt/UpperLeg_Right_jnt"
   :right-arm "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Right_jnt"
   :right-elbow "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Right_jnt/Arm_Right_jnt"
   :left-arm "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Left_jnt"
   :left-elbow "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Shoulder_Left_jnt/Arm_Left_jnt"
   :head "Hips_jnt/Spine_jnt/Spine_jnt 1/Chest_jnt/Neck_jnt/Head_jnt"
   :middle-spine "Hips_jnt/Spine_jnt/Spine_jnt 1"
   :pelvis "Hips_jnt"})    

(comment
  (ragdoll!
    {:total-mass 10
     :head (object-named "Head_M")
     :middle-spine (object-named "Spine1_M")
     :pelvis (object-named "Root_M")
     :left-hips (object-named "Hip_L")
     :left-knee (object-named "Knee_L")
     :left-foot (object-named "Ankle_L")
     :left-elbow (object-named "Elbow_L")
     :left-arm (object-named "Shoulder_L")
     :right-arm (object-named "Shoulder_R")
     :right-hips (object-named "Hip_R")
     :right-knee (object-named "Knee_R")
     :right-elbow (object-named "Elbow_R")
     :right-foot (object-named "Ankle_R")})
  
  (ragmap
    Selection/activeObject
    {:left-foot "Group/Main/DeformationSystem/Root_M/Hip_L/Knee_L/Ankle_L"
     :left-hips "Group/Main/DeformationSystem/Root_M/Hip_L"
     :right-hips "Group/Main/DeformationSystem/Root_M/Hip_R"
     :right-foot "Group/Main/DeformationSystem/Root_M/Hip_R/Knee_R/Ankle_R"
     :right-arm "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Scapula_R/Shoulder_R"
     :right-elbow "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R"
     :head "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Neck_M/Head_M"
     :right-knee "Group/Main/DeformationSystem/Root_M/Hip_R/Knee_R"
     :left-knee "Group/Main/DeformationSystem/Root_M/Hip_L/Knee_L"
     :left-elbow "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Scapula_L/Shoulder_L/Elbow_L"
     :middle-spine "Group/Main/DeformationSystem/Root_M/Spine1_M"
     :left-arm "Group/Main/DeformationSystem/Root_M/Spine1_M/Spine1Part1_M/Spine1Part2_M/Chest_M/Scapula_L/Shoulder_L"
     :pelvis "Group/Main/DeformationSystem/Root_M"}))

(comment
  (doseq [dude Selection/objects]
    (-> dude
        (ragmap simple)
        ragdoll!))
  
  (UnityEngine.Application/LoadLevel (UnityEngine.Application/loadedLevel))
  (do 
    (doseq [col (.GetComponentsInChildren Selection/activeObject Collider)]
      (GameObject/DestroyImmediate col))
    (doseq [col (.GetComponentsInChildren Selection/activeObject Joint)]
      (GameObject/DestroyImmediate col))
    (doseq [col (.GetComponentsInChildren Selection/activeObject Rigidbody)]
      (GameObject/DestroyImmediate col)))
  (require 'game.ragdoll :reload)
  )