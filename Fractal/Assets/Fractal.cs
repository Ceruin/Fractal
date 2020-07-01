using UnityEngine;
using System.Collections;

// ---------------------------------------------------------------------
//                                  Fractal Script
// This script is a recursive script that will create new children
// the children created then run through the same script and
// create children of their own. This continues based on our
// variables.
// ---------------------------------------------------------------------
public class Fractal : MonoBehaviour {

    // ---------------------------------------------------------------------
    //                                  Possible Child Directions
    // These directions are representing the possible directions on a
    // 3d plane, a 3d plane in unity has an X, Y, and Z.
    // As you could expect the vector3's variable is an array of the
    // basic directions the child will use
    // ---------------------------------------------------------------------
    private static Vector3[] childDirections = {
		Vector3.up,
		Vector3.right,
		Vector3.left,
		Vector3.forward,
		Vector3.back
	};

    // ---------------------------------------------------------------------
    //                                  Possible Child Rotations
    // In c#/unity a Quaternion is essentially a representation of an
    // objects rotation.
    // Similar to the directions we have Eulers which are rotations
    // that rotate around the respective x, y, z axis's in the world
    // ---------------------------------------------------------------------
    private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f)
	};

    // ---------------------------------------------------------------------
    //                                  "Public" Variables
    // The SerializeField is similar to making our variable public,
    // this allows the unity engine to have components dragged/modifed
    // from or to the inspector.
    // ---------------------------------------------------------------------
    [SerializeField] Mesh[] meshes;
    [SerializeField] Material material;
    [SerializeField] int maxDepth;
    [SerializeField] float childScale;
    [SerializeField] float spawnProbability;
    [SerializeField] float maxRotationSpeed;
    [SerializeField] float maxTwist;

    // ---------------------------------------------------------------------
    //                                  Private Variables
    // These are basic private variables only accessible to the Fractal cs
    // ---------------------------------------------------------------------
    private float rotationSpeed;
	private int depth;
	private Material[,] materials;

    // ---------------------------------------------------------------------
    //                                  Material Initializer
    // In unity our 3d objects have materials and colors, this is to create
    // and setup the colors and material used for our new fractal objects
    // ---------------------------------------------------------------------
    private void InitializeMaterials () {
		materials = new Material[maxDepth + 1, 2];
		for (int i = 0; i <= maxDepth; i++) {
			float t = i / (maxDepth - 1f);
			t *= t;
			materials[i, 0] = new Material(material);
			materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
			materials[i, 1] = new Material(material);
			materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
		}
		materials[maxDepth, 0].color = Color.magenta;
		materials[maxDepth, 1].color = Color.red;
	}

    // ---------------------------------------------------------------------
    //                                  Start
    // This is a method that starts when the object has been created,
    // essentially it is an initialization of our script.
    // ---------------------------------------------------------------------
    private void Start() {
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
        if (materials == null) {
            InitializeMaterials();
        }
        gameObject.AddComponent<MeshFilter>().mesh =
            meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material =
            materials[depth, Random.Range(0, 2)];
        if (depth < maxDepth) {
            StartCoroutine(CreateChildren());
        }
    }

    // ---------------------------------------------------------------------
    //                                  Children Creation Coroutine
    // This is a c# coroutine for creating the child fractals.
    // We can claim this is where the bulk of our recursion happens as it
    // initializes and spawns the fractals which will then spawn their own.
    // Essentially the children create more children.
    // ---------------------------------------------------------------------
    private IEnumerator CreateChildren () {
		for (int i = 0; i < childDirections.Length; i++) {
			if (Random.value < spawnProbability) {
				yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
				new GameObject("Fractal Child").AddComponent<Fractal>().
					Initialize(this, i);
			}
		}
	}

    // ---------------------------------------------------------------------
    //                                  Initializer
    // This sets the properties of our child as called by the coroutine.
    // As you can see some of the properties are inherited from the
    // parent/previous fractal.
    // ---------------------------------------------------------------------
    private void Initialize (Fractal parent, int childIndex) {
		meshes = parent.meshes;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		spawnProbability = parent.spawnProbability;
		maxRotationSpeed = parent.maxRotationSpeed;
		maxTwist = parent.maxTwist;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition =
			childDirections[childIndex] * (0.5f + 0.5f * childScale);
		transform.localRotation = childOrientations[childIndex];
	}

    // ---------------------------------------------------------------------
    //                                  Update
    // An update in unity is called every frame, this means that this is
    // constantly setting the transform(a container of the objects position,
    // rotation and scale) to rotate at a set speed.
    // ---------------------------------------------------------------------
    private void Update () {
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
	}
}