using System.Collections;
using UnityEngine;

public class RagdollBuilder
{
  //
  // Fields
  //
  public Vector3 worldRight = Vector3.right;

  public Transform head;

  public float totalMass = 20f;

  public float strength;

  public Vector3 right = Vector3.right;

  public Vector3 up = Vector3.up;

  public Vector3 forward = Vector3.forward;

  public Vector3 worldUp = Vector3.up;

  public Vector3 worldForward = Vector3.forward;

  public bool flipForward;

  public ArrayList bones;

  public RagdollBuilder.BoneInfo rootBone;

  public Transform middleSpine;

  public Transform pelvis;

  public Transform leftHips;

  public Transform leftKnee;

  public Transform leftFoot;

  public Transform rightHips;

  public Transform rightKnee;

  public Transform rightElbow;

  public Transform rightArm;

  public Transform leftElbow;

  public Transform leftArm;

  public Transform rightFoot;

    //
    // Static Methods
    //
  public static void CalculateDirection (Vector3 point, out int direction, out float distance)
  {
    direction = 0;
    if (Mathf.Abs (point [1]) > Mathf.Abs (point [0])) {
      direction = 1;
    }
    if (Mathf.Abs (point [2]) > Mathf.Abs (point [direction])) {
      direction = 2;
    }
    distance = point [direction];
  }

  public static Vector3 CalculateDirectionAxis (Vector3 point)
  {
    int index = 0;
    float num;
    RagdollBuilder.CalculateDirection (point, out index, out num);
    Vector3 zero = Vector3.zero;
    if (num > 0f) {
      zero [index] = 1f;
    }
    else {
      zero [index] = -1f;
    }
    return zero;
  }

  public static int LargestComponent (Vector3 point)
  {
    int num = 0;
    if (Mathf.Abs (point [1]) > Mathf.Abs (point [0])) {
      num = 1;
    }
    if (Mathf.Abs (point [2]) > Mathf.Abs (point [num])) {
      num = 2;
    }
    return num;
  }

  public static int SecondLargestComponent (Vector3 point)
  {
    int num = RagdollBuilder.SmallestComponent (point);
    int num2 = RagdollBuilder.LargestComponent (point);
    if (num < num2) {
      int num3 = num2;
      num2 = num;
      num = num3;
    }
    if (num == 0 && num2 == 1) {
      return 2;
    }
    if (num == 0 && num2 == 2) {
      return 1;
    }
    return 0;
  }

  public static int SmallestComponent (Vector3 point)
  {
    int num = 0;
    if (Mathf.Abs (point [1]) < Mathf.Abs (point [0])) {
      num = 1;
    }
    if (Mathf.Abs (point [2]) < Mathf.Abs (point [num])) {
      num = 2;
    }
    return num;
  }

    //
    // Methods
    //
  public void AddBreastColliders ()
  {
    if (this.middleSpine != null && this.pelvis != null) {
      Bounds bounds = this.Clip (this.GetBreastBounds (this.pelvis), this.pelvis, this.middleSpine, false);
      BoxCollider boxCollider = this.pelvis.gameObject.AddComponent<BoxCollider> ();
      boxCollider.center = bounds.center;
      boxCollider.size = bounds.size;
      bounds = this.Clip (this.GetBreastBounds (this.middleSpine), this.middleSpine, this.middleSpine, true);
      boxCollider = this.middleSpine.gameObject.AddComponent<BoxCollider> ();
      boxCollider.center = bounds.center;
      boxCollider.size = bounds.size;
    }
    else {
      Bounds bounds2 = default(Bounds);
      bounds2.Encapsulate (this.pelvis.InverseTransformPoint (this.leftHips.position));
      bounds2.Encapsulate (this.pelvis.InverseTransformPoint (this.rightHips.position));
      bounds2.Encapsulate (this.pelvis.InverseTransformPoint (this.leftArm.position));
      bounds2.Encapsulate (this.pelvis.InverseTransformPoint (this.rightArm.position));
      Vector3 size = bounds2.size;
      size [RagdollBuilder.SmallestComponent (bounds2.size)] = size [RagdollBuilder.LargestComponent (bounds2.size)] / 2f;
      BoxCollider boxCollider2 = this.pelvis.gameObject.AddComponent<BoxCollider> ();
      boxCollider2.center = bounds2.center;
      boxCollider2.size = size;
    }
  }

  public void AddHeadCollider ()
  {
    if (this.head.GetComponent<Collider> ()) {
      Object.Destroy (this.head.GetComponent<Collider> ());
    }
    float num = Vector3.Distance (this.leftArm.transform.position, this.rightArm.transform.position);
    num /= 4f;
    SphereCollider sphereCollider = this.head.gameObject.AddComponent<SphereCollider> ();
    sphereCollider.radius = num;
    Vector3 zero = Vector3.zero;
    int index;
    float num2;
    RagdollBuilder.CalculateDirection (this.head.InverseTransformPoint (this.pelvis.position), out index, out num2);
    if (num2 > 0f) {
      zero [index] = -num;
    }
    else {
      zero [index] = num;
    }
    sphereCollider.center = zero;
  }

  public void AddJoint (string name, Transform anchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, System.Type colliderType, float radiusScale, float density)
  {
    RagdollBuilder.BoneInfo boneInfo = new RagdollBuilder.BoneInfo ();
    boneInfo.name = name;
    boneInfo.anchor = anchor;
    boneInfo.axis = worldTwistAxis;
    boneInfo.normalAxis = worldSwingAxis;
    boneInfo.minLimit = minLimit;
    boneInfo.maxLimit = maxLimit;
    boneInfo.swingLimit = swingLimit;
    boneInfo.density = density;
    boneInfo.colliderType = colliderType;
    boneInfo.radiusScale = radiusScale;
    if (this.FindBone (parent) != null) {
      boneInfo.parent = this.FindBone (parent);
    }
    else if (name.StartsWith ("Left")) {
      boneInfo.parent = this.FindBone ("Left " + parent);
    }
    else if (name.StartsWith ("Right")) {
      boneInfo.parent = this.FindBone ("Right " + parent);
    }
    boneInfo.parent.children.Add (boneInfo);
    this.bones.Add (boneInfo);
  }

  public void AddMirroredJoint (string name, Transform leftAnchor, Transform rightAnchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, System.Type colliderType, float radiusScale, float density)
  {
    this.AddJoint ("Left " + name, leftAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
    this.AddJoint ("Right " + name, rightAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
  }

  public void BuildBodies ()
  {
    foreach (RagdollBuilder.BoneInfo boneInfo in this.bones) {
      boneInfo.anchor.gameObject.AddComponent<Rigidbody> ();
      boneInfo.anchor.GetComponent<Rigidbody> ().mass = boneInfo.density;
    }
  }

  public void BuildCapsules ()
  {
    foreach (RagdollBuilder.BoneInfo boneInfo in this.bones) {
      if (boneInfo.colliderType == typeof(CapsuleCollider)) {
        int num;
        float num2;
        if (boneInfo.children.Count == 1) {
          RagdollBuilder.BoneInfo boneInfo2 = (RagdollBuilder.BoneInfo)boneInfo.children [0];
          Vector3 position = boneInfo2.anchor.position;
          RagdollBuilder.CalculateDirection (boneInfo.anchor.InverseTransformPoint (position), out num, out num2);
        }
        else {
          Vector3 position2 = boneInfo.anchor.position - boneInfo.parent.anchor.position + boneInfo.anchor.position;
          RagdollBuilder.CalculateDirection (boneInfo.anchor.InverseTransformPoint (position2), out num, out num2);
          if (boneInfo.anchor.GetComponentsInChildren (typeof(Transform)).Length > 1) {
            Bounds bounds = default(Bounds);
            Component[] componentsInChildren = boneInfo.anchor.GetComponentsInChildren (typeof(Transform));
            for (int i = 0; i < componentsInChildren.Length; i++) {
              Transform transform = (Transform)componentsInChildren [i];
              bounds.Encapsulate (boneInfo.anchor.InverseTransformPoint (transform.position));
            }
            if (num2 > 0f) {
              num2 = bounds.max [num];
            }
            else {
              num2 = bounds.min [num];
            }
          }
        }
        CapsuleCollider capsuleCollider = boneInfo.anchor.gameObject.AddComponent<CapsuleCollider> ();
        capsuleCollider.direction = num;
        Vector3 zero = Vector3.zero;
        zero [num] = num2 * 0.5f;
        capsuleCollider.center = zero;
        capsuleCollider.height = Mathf.Abs (num2);
        capsuleCollider.radius = Mathf.Abs (num2 * boneInfo.radiusScale);
      }
    }
  }

  public void BuildJoints ()
  {
    foreach (RagdollBuilder.BoneInfo boneInfo in this.bones) {
      if (boneInfo.parent != null) {
        CharacterJoint characterJoint = boneInfo.anchor.gameObject.AddComponent<CharacterJoint> ();
        boneInfo.joint = characterJoint;
        characterJoint.axis = RagdollBuilder.CalculateDirectionAxis (boneInfo.anchor.InverseTransformDirection (boneInfo.axis));
        characterJoint.swingAxis = RagdollBuilder.CalculateDirectionAxis (boneInfo.anchor.InverseTransformDirection (boneInfo.normalAxis));
        characterJoint.anchor = Vector3.zero;
        characterJoint.connectedBody = boneInfo.parent.anchor.GetComponent<Rigidbody> ();
        characterJoint.enablePreprocessing = false;
        SoftJointLimit softJointLimit = default(SoftJointLimit);
        softJointLimit.contactDistance = 0f;
        softJointLimit.limit = boneInfo.minLimit;
        characterJoint.lowTwistLimit = softJointLimit;
        softJointLimit.limit = boneInfo.maxLimit;
        characterJoint.highTwistLimit = softJointLimit;
        softJointLimit.limit = boneInfo.swingLimit;
        characterJoint.swing1Limit = softJointLimit;
        softJointLimit.limit = 0f;
        characterJoint.swing2Limit = softJointLimit;
      }
    }
  }

  public void CalculateAxes ()
  {
    if (this.head != null && this.pelvis != null) {
      this.up = RagdollBuilder.CalculateDirectionAxis (this.pelvis.InverseTransformPoint (this.head.position));
    }
    if (this.rightElbow != null && this.pelvis != null) {
      Vector3 vector;
      Vector3 point;
      this.DecomposeVector (out vector, out point, this.pelvis.InverseTransformPoint (this.rightElbow.position), this.up);
      this.right = RagdollBuilder.CalculateDirectionAxis (point);
    }
    this.forward = Vector3.Cross (this.right, this.up);
    if (this.flipForward) {
      this.forward = -this.forward;
    }
  }

  public void CalculateMass ()
  {
    this.CalculateMassRecurse (this.rootBone);
    float num = this.totalMass / this.rootBone.summedMass;
    foreach (RagdollBuilder.BoneInfo boneInfo in this.bones) {
      boneInfo.anchor.GetComponent<Rigidbody> ().mass *= num;
    }
    this.CalculateMassRecurse (this.rootBone);
  }

  public void CalculateMassRecurse (RagdollBuilder.BoneInfo bone)
  {
    float num = bone.anchor.GetComponent<Rigidbody> ().mass;
    foreach (RagdollBuilder.BoneInfo boneInfo in bone.children) {
      this.CalculateMassRecurse (boneInfo);
      num += boneInfo.summedMass;
    }
    bone.summedMass = num;
  }

  public string CheckConsistency ()
  {
    this.PrepareBones ();
    Hashtable hashtable = new Hashtable ();
    foreach (RagdollBuilder.BoneInfo boneInfo in this.bones) {
      if (boneInfo.anchor) {
        if (hashtable [boneInfo.anchor] != null) {
          RagdollBuilder.BoneInfo boneInfo2 = (RagdollBuilder.BoneInfo)hashtable [boneInfo.anchor];
          string result = string.Format ("{0} and {1} may not be assigned to the same bone.", boneInfo.name, boneInfo2.name);
          return result;
        }
        hashtable [boneInfo.anchor] = boneInfo;
      }
    }
    foreach (RagdollBuilder.BoneInfo boneInfo3 in this.bones) {
      if (boneInfo3.anchor == null) {
        string result = string.Format ("{0} has not been assigned yet.\n", boneInfo3.name);
        return result;
      }
    }
    return string.Empty;
  }

  public void Cleanup ()
  {
    foreach (RagdollBuilder.BoneInfo boneInfo in this.bones) {
      if (boneInfo.anchor) {
        Component[] componentsInChildren = boneInfo.anchor.GetComponentsInChildren (typeof(Joint));
        Component[] array = componentsInChildren;
        for (int i = 0; i < array.Length; i++) {
          Joint obj = (Joint)array [i];
          Object.DestroyImmediate (obj);
        }
        Component[] componentsInChildren2 = boneInfo.anchor.GetComponentsInChildren (typeof(Rigidbody));
        Component[] array2 = componentsInChildren2;
        for (int j = 0; j < array2.Length; j++) {
          Rigidbody obj2 = (Rigidbody)array2 [j];
          Object.DestroyImmediate (obj2);
        }
        Component[] componentsInChildren3 = boneInfo.anchor.GetComponentsInChildren (typeof(Collider));
        Component[] array3 = componentsInChildren3;
        for (int k = 0; k < array3.Length; k++) {
          Collider obj3 = (Collider)array3 [k];
          Object.DestroyImmediate (obj3);
        }
      }
    }
  }

  public Bounds Clip (Bounds bounds, Transform relativeTo, Transform clipTransform, bool below)
  {
    int index = RagdollBuilder.LargestComponent (bounds.size);
    if (Vector3.Dot (this.worldUp, relativeTo.TransformPoint (bounds.max)) > Vector3.Dot (this.worldUp, relativeTo.TransformPoint (bounds.min)) == below) {
      Vector3 min = bounds.min;
      min [index] = relativeTo.InverseTransformPoint (clipTransform.position) [index];
      bounds.min = min;
    }
    else {
      Vector3 max = bounds.max;
      max [index] = relativeTo.InverseTransformPoint (clipTransform.position) [index];
      bounds.max = max;
    }
    return bounds;
  }

  public void DecomposeVector (out Vector3 normalCompo, out Vector3 tangentCompo, Vector3 outwardDir, Vector3 outwardNormal)
  {
    outwardNormal = outwardNormal.normalized;
    normalCompo = outwardNormal * Vector3.Dot (outwardDir, outwardNormal);
    tangentCompo = outwardDir - normalCompo;
  }

  public RagdollBuilder.BoneInfo FindBone (string name)
  {
    foreach (RagdollBuilder.BoneInfo boneInfo in this.bones) {
      if (boneInfo.name == name) {
        return boneInfo;
      }
    }
    return null;
  }

  public Bounds GetBreastBounds (Transform relativeTo)
  {
    Bounds result = default(Bounds);
    result.Encapsulate (relativeTo.InverseTransformPoint (this.leftHips.position));
    result.Encapsulate (relativeTo.InverseTransformPoint (this.rightHips.position));
    result.Encapsulate (relativeTo.InverseTransformPoint (this.leftArm.position));
    result.Encapsulate (relativeTo.InverseTransformPoint (this.rightArm.position));
    Vector3 size = result.size;
    size [RagdollBuilder.SmallestComponent (result.size)] = size [RagdollBuilder.LargestComponent (result.size)] / 2f;
    result.size = size;
    return result;
  }

  public void OnDrawGizmos ()
  {
    if (this.pelvis) {
      Gizmos.color = Color.red;
      Gizmos.DrawRay (this.pelvis.position, this.pelvis.TransformDirection (this.right));
      Gizmos.color = Color.green;
      Gizmos.DrawRay (this.pelvis.position, this.pelvis.TransformDirection (this.up));
      Gizmos.color = Color.blue;
      Gizmos.DrawRay (this.pelvis.position, this.pelvis.TransformDirection (this.forward));
    }
  }

  public void OnWizardCreate ()
  {
    this.Cleanup ();
    this.BuildCapsules ();
    this.AddBreastColliders ();
    this.AddHeadCollider ();
    this.BuildBodies ();
    this.BuildJoints ();
    this.CalculateMass ();
  }

  public void PrepareBones ()
  {
    if (this.pelvis) {
      this.worldRight = this.pelvis.TransformDirection (this.right);
      this.worldUp = this.pelvis.TransformDirection (this.up);
      this.worldForward = this.pelvis.TransformDirection (this.forward);
    }
    this.bones = new ArrayList ();
    this.rootBone = new RagdollBuilder.BoneInfo ();
    this.rootBone.name = "Pelvis";
    this.rootBone.anchor = this.pelvis;
    this.rootBone.parent = null;
    this.rootBone.density = 2.5f;
    this.bones.Add (this.rootBone);
    this.AddMirroredJoint ("Hips", this.leftHips, this.rightHips, "Pelvis", this.worldRight, this.worldForward, -20f, 70f, 30f, typeof(CapsuleCollider), 0.3f, 1.5f);
    this.AddMirroredJoint ("Knee", this.leftKnee, this.rightKnee, "Hips", this.worldRight, this.worldForward, -80f, 0f, 0f, typeof(CapsuleCollider), 0.25f, 1.5f);
    this.AddJoint ("Middle Spine", this.middleSpine, "Pelvis", this.worldRight, this.worldForward, -20f, 20f, 10f, null, 1f, 2.5f);
    this.AddMirroredJoint ("Arm", this.leftArm, this.rightArm, "Middle Spine", this.worldUp, this.worldForward, -70f, 10f, 50f, typeof(CapsuleCollider), 0.25f, 1f);
    this.AddMirroredJoint ("Elbow", this.leftElbow, this.rightElbow, "Arm", this.worldForward, this.worldUp, -90f, 0f, 0f, typeof(CapsuleCollider), 0.2f, 1f);
    this.AddJoint ("Head", this.head, "Middle Spine", this.worldRight, this.worldForward, -40f, 25f, 25f, null, 1f, 1f);
  }

    //
    // Nested Types
    //
  public class BoneInfo
  {
    public string name;

    public Transform anchor;

    public CharacterJoint joint;

    public RagdollBuilder.BoneInfo parent;

    public float minLimit;

    public float maxLimit;

    public float swingLimit;

    public Vector3 axis;

    public Vector3 normalAxis;

    public float radiusScale;

    public System.Type colliderType;

    public ArrayList children = new ArrayList ();

    public float density;

    public float summedMass;
  }
}
