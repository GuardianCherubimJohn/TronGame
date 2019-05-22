using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildQuad
{
    static Vector3 GetTriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 edge1 = b - a;
        Vector3 edge2 = c - a;
        Vector3 normal = Vector3.Cross(edge1, edge2);
        return normal;
    }

    public static Quad Build(Vector3 bl, Vector3 ul, Vector3 br, Vector3 ur, int triangleStart, Vector3 normal) {

        Quad quad = new Quad();
        quad.Vertices = new Vector3[4];

        quad.Vertices[0] = bl;
        quad.Vertices[1] = br;
        quad.Vertices[2] = ul;
        quad.Vertices[3] = ur;

        quad.Triangles = new int[6];

        quad.Triangles[0] = 0 + triangleStart;
        quad.Triangles[1] = 2 + triangleStart;
        quad.Triangles[2] = 1 + triangleStart;

        quad.Triangles[3] = 2 + triangleStart;
        quad.Triangles[4] = 3 + triangleStart;
        quad.Triangles[5] = 1 + triangleStart;

        quad.Normals = new Vector3[4];

        quad.Normals[0] = normal;
        quad.Normals[1] = normal;
        quad.Normals[2] = normal;
        quad.Normals[3] = normal;

        quad.UV = new Vector2[4];

        quad.UV[0] = new Vector2(0, 0);
        quad.UV[1] = new Vector2(1, 0);
        quad.UV[2] = new Vector2(0, 1);
        quad.UV[3] = new Vector2(1, 1);

        return quad;
    }

    public static Mesh BuildMesh(Mesh mesh, Box[] boxes) {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        foreach(Box b in boxes) {
            foreach (Quad q in b.Quads)
            {
                verts.AddRange(q.Vertices);
                tris.AddRange(q.Triangles);
                normals.AddRange(q.Normals);
                uvs.AddRange(q.UV);
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        return mesh;
    }

    public class Quad {
        public Vector3[] Vertices {get;set;}
        public int[] Triangles {get;set;}
        public Vector3[] Normals {get;set;}
        public Vector2[] UV {get;set;}
    }

    public class Box {
        public Quad front { get; set; }
        public Quad back { get; set; }
        public Quad left { get; set; }
        public Quad right { get; set; }
        public Quad up { get; set; }
        public Quad down { get; set; }

        public Quad[] Quads { get {
                return new Quad[6] { front, back, left, right, up, down };
            }}
    }
}
