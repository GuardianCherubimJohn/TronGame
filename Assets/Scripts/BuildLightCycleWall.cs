using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class BuildLightCycleWall : MonoBehaviour {

    public Transform LightCycle;
    public List<Vector3> CyclePositions;
    public LineRenderer Rend;
    public LightCyclePlayerMovement Player;

    public MeshFilter meshFilter;
    Mesh wallMesh;
    List<Vector3> vertices;

    public float Thickness = 1f;
    public float Height = 1f;
    LightCyclePlayerMovement.RotationArgs test;
    bool isLocked = false;

    TaskScheduler mainThread;

    private void Awake()
    {
        //Application.targetFrameRate = 24;
    }

    int triangleStart = 0;
    List<BuildQuad.Box> boxes = new List<BuildQuad.Box>();

    // Use this for initialization
    void Start () {
        wallMesh = new Mesh();
        TaskScheduler sched = TaskScheduler.FromCurrentSynchronizationContext();
        mainThread = sched;
        //Debug.Log(mainThread);
        CyclePositions = new List<Vector3>();
        CyclePositions.Add(LightCycle.position);
        BuildExistingMesh();

        Player.PlayerRotationEvent += (object sender, LightCyclePlayerMovement.RotationArgs e) => {
            Task mytask = new Task(() => {
                CyclePositions.Add(LightCycle.position);
                BuildExistingMesh();
            });
            mytask.Start(mainThread);
        };
	}

    BuildQuad.Box CubeThickness(Vector3 start, Vector3 end, float thickness, float height, int triangleStart) {
        
        Vector3 direction = end - start;
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        if (direction != Vector3.zero)
            rotation = Quaternion.LookRotation(direction);
        Quaternion right = rotation * Quaternion.Euler(new Vector3(0f, 90f, 0f));
        Quaternion left = rotation * Quaternion.Euler(new Vector3(0f, -90f, 0f));
        Quaternion up = rotation * Quaternion.Euler(new Vector3(90f, 0f, 0f));
        Quaternion down = rotation * Quaternion.Euler(new Vector3(-90f, 0f, 0f));

        Vector3 leftdirection = left * Vector3.forward;
        Vector3 rightdirection = right * Vector3.forward;
        Vector3 updirection = up * Vector3.forward;
        Vector3 downdirection = down * Vector3.forward;

        Vector3 backdirection = -1f * direction;

        Vector3 sbl = start + (leftdirection * thickness);
        Vector3 sbr = start + (rightdirection * thickness);
        Vector3 sul = start + (leftdirection * thickness) + new Vector3(0f, height, 0f);
        Vector3 sur = start + (rightdirection * thickness) + new Vector3(0f, height, 0f);

        Vector3 ebl = end + (leftdirection * thickness);
        Vector3 ebr = end + (rightdirection * thickness);
        Vector3 eul = end + (leftdirection * thickness) + new Vector3(0f, height, 0f);
        Vector3 eur = end + (rightdirection * thickness) + new Vector3(0f, height, 0f);

        BuildQuad.Quad qk = BuildQuad.Build(sbl, sul, sbr, sur, triangleStart, backdirection);
        BuildQuad.Quad qf = BuildQuad.Build(ebr, eur, ebl, eul, triangleStart + 4, direction);
        BuildQuad.Quad ql = BuildQuad.Build(ebl, eul, sbl, sul, triangleStart + 8, leftdirection);
        BuildQuad.Quad qr = BuildQuad.Build(sbr, sur, ebr, eur, triangleStart + 12, rightdirection);

        BuildQuad.Quad qt = BuildQuad.Build(sul, eul, sur, eur, triangleStart + 16, updirection);
        BuildQuad.Quad qb = BuildQuad.Build(sbl, ebl, sbr, ebr, triangleStart + 20, downdirection);

        return new BuildQuad.Box { 
            back = qk, 
            front = qf,
            left = ql, 
            right = qr, 
            up = qt, 
            down = qb };
    }

    void BuildExistingMesh() {
        triangleStart = 0;
        boxes = new List<BuildQuad.Box>();
        for (int z = 0; z < CyclePositions.Count - 1; z++)
        {
            boxes.Add(CubeThickness(CyclePositions[z], CyclePositions[z + 1], Thickness, Height, triangleStart));
            triangleStart += 24;
        }
        boxes.Add(CubeThickness(CyclePositions[CyclePositions.Count - 1], LightCycle.position, Thickness, Height, triangleStart));
    }

    void SetupMesh() {
        boxes[CyclePositions.Count - 1] = CubeThickness(CyclePositions[CyclePositions.Count - 1], LightCycle.position, Thickness, Height, triangleStart);
        wallMesh = BuildQuad.BuildMesh(wallMesh, boxes.ToArray());
        wallMesh.bounds = new Bounds(Vector3.zero, new Vector3(1000f, 1000f, 1000f));
        meshFilter.mesh = wallMesh;
    }
	
	// Update is called once per frame
	void Update () {
        SetupMesh();
	}
}
