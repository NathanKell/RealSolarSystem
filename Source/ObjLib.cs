// ObjExporter and ObjImporter based on code by KeliHlodversson and  el anónimo respectively from the Unity wiki

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RealSolarSystem
{
    class ObjLib
    {
        // **** VERTEX MATCH ****
        public static void UpdateMeshFromFile(Mesh mesh, string filename, float scaleFactor = 1.0f)
        {
            ProfileTimer.Push("UpdateMeshFromFile");
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector4> tangents = new List<Vector4>();
            List<Vector2> uv = new List<Vector2>();
            using (StreamReader stream = File.OpenText(filename))
            {
                stream.ReadLine();
                string curLine = stream.ReadLine();
                char[] splitIdentifier = { ' ' };
                string[] brokenString;

                while (curLine != null)
                {
                    curLine = curLine.Trim();                           //Trim the current line
                    brokenString = curLine.Split(splitIdentifier, 50);      //Split the line into an array, separating the original line by blank spaces
                    //if(log) MonoBehaviour.print(curLine);
                    switch (brokenString[0])
                    {
                        case "v":
                            vertices.Add(new Vector3(System.Convert.ToSingle(brokenString[1]) * scaleFactor, System.Convert.ToSingle(brokenString[2]) * scaleFactor,
                                System.Convert.ToSingle(brokenString[3]) * scaleFactor));
                            break;
                        case "vn":
                            normals.Add(new Vector3(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]),
                                System.Convert.ToSingle(brokenString[3])));
                            break;
                        case "t":
                            tangents.Add(new Vector4(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]),
                                System.Convert.ToSingle(brokenString[3]), System.Convert.ToSingle(brokenString[4])));
                            break;
                        case "vt":
                            uv.Add(new Vector2(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2])));
                            break;
                    }
                    curLine = stream.ReadLine();
                    if (curLine != null)
                    {
                        curLine = curLine.Replace("  ", " ");
                    }
                }
            }
            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.tangents = tangents.ToArray();
            mesh.uv = uv.ToArray();
            ProfileTimer.Pop("UpdateMeshFromFile");
        }
        // based on noontz's code here: http://forum.unity3d.com/threads/38984-How-to-Calculate-Mesh-Tangents
        public static void UpdateTangents(Mesh mesh)
        {
            ProfileTimer.Push("UpdateTangents");
            Vector4[] tangents = new Vector4[mesh.vertexCount];
            int triangleCount = mesh.triangles.Length / 3;
            Vector3[] tan1 = new Vector3[mesh.vertexCount];
            Vector3[] tan2 = new Vector3[mesh.vertexCount];
            int tri = 0;
            Vector3 sdir = new Vector3();
            Vector3 tdir = new Vector3();

            for (int i = 0; i < triangleCount; i++)
            {

                int i1 = mesh.triangles[tri];
                int i2 = mesh.triangles[tri + 1];
                int i3 = mesh.triangles[tri + 2];

                Vector3 v1 = mesh.vertices[i1];
                Vector3 v2 = mesh.vertices[i2];
                Vector3 v3 = mesh.vertices[i3];

                Vector2 w1 = mesh.uv[i1];
                Vector2 w2 = mesh.uv[i2];
                Vector2 w3 = mesh.uv[i3];


                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = 1.0f / (s1 * t2 - s2 * t1);

                sdir.x = (t2 * x1 - t1 * x2);
                sdir.y = (t2 * y1 - t1 * y2);
                sdir.z = (t2 * z1 - t1 * z2);
                sdir *= r;

                tdir.x = (s1 * x2 - s2 * x1);
                tdir.y = (s1 * y2 - s2 * y1);
                tdir.z = (s1 * z2 - s2 * z1);
                tdir *= r;

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;

                tri += 3;
            }

            for (int i = 0; i < mesh.vertexCount; i++)
            {
                Vector3 n = mesh.normals[i];
                Vector3 t = tan1[i];

                // Gram-Schmidt orthogonalize
                Vector3.OrthoNormalize(ref n, ref t);

                tangents[i].x = t.x;
                tangents[i].y = t.y;
                tangents[i].z = t.z;

                // Calculate handedness
                tangents[i].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;

            }

            mesh.tangents = tangents;
            ProfileTimer.Pop("UpdateTangents");
        }
        public static void VertsTangentsToFile(MeshFilter mf, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write("g " + mf.name + "\n");
                foreach (Vector3 v in mf.mesh.vertices)
                    sw.Write(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));

                foreach(Vector4 t in mf.mesh.tangents)
                    sw.Write(string.Format("t {0} {1} {2} {3}\n", t.x, t.y, t.z, t.w));
            }
        }
        // **** EXPORT ****
        public static string MeshToString(MeshFilter mf)
        {
            Mesh m = mf.mesh;

            MonoBehaviour.print(string.Format("About to save {0}: {1} verts, {2} normals, {3} tangents, {4} uvs",
                Path.GetFileNameWithoutExtension(mf.name),
                m.vertices.Length,
                m.normals.Length,
                m.tangents.Length,
                m.uv.Length));

            Material[] mats = mf.renderer.sharedMaterials;

            StringBuilder sb = new StringBuilder();

            sb.Append("g ").Append(mf.name).Append("\n");
            for(int i = 0; i < m.vertexCount; i++)
            {
                Vector3 v = m.vertices[i];
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            for(int i = 0; i < m.normals.Length; i++)
            {
                Vector3 v = m.normals[i];
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            for (int i = 0; i < m.uv.Length; i++)
            {
                Vector2 v = m.uv[i];
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            sb.Append("\n"); // extend to include tangent output
            for (int i = 0; i < m.tangents.Length; i++ )
            {
                Vector4 t = m.tangents[i];
                sb.Append(string.Format("t {0} {1} {2} {3}\n", t.x, t.y, t.z, t.w));
            }
            for (int material = 0; material < m.subMeshCount; material++)
            {
                sb.Append("\n");
                sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                sb.Append("usemap ").Append(mats[material].name).Append("\n");

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }
            return sb.ToString();
        }

        public static void MeshToFile(MeshFilter mf, string filename)
        {
            ProfileTimer.Pop("Mesh to file");
            var dirInfo = Path.GetDirectoryName(filename);
            if (!Directory.Exists(dirInfo))
                Directory.CreateDirectory(dirInfo);
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(MeshToString(mf));
            }
            ProfileTimer.Pop("Mesh to file");
        }

        // **** IMPORT ****

        private struct meshStruct
        {
            public Vector3[] vertices;
            public Vector3[] normals;
            public Vector2[] uv;
            public int[] triangles;
            public Vector3[] faceData;
            public string fileName;
        }

        // Use this for initialization
        public static Mesh FileToMesh(string filePath)
        {
            meshStruct newMesh = createMeshStruct(filePath);
            populateMeshStruct(ref newMesh);

            Vector3[] newVerts = new Vector3[newMesh.faceData.Length];
            Vector2[] newUVs = new Vector2[newMesh.faceData.Length];
            Vector3[] newNormals = new Vector3[newMesh.faceData.Length];
            int i = 0;
            /* The following foreach loops through the facedata and assigns the appropriate vertex, uv, or normal
             * for the appropriate Unity mesh array.
             */
            foreach (Vector3 v in newMesh.faceData)
            {
                newVerts[i] = newMesh.vertices[(int)v.x - 1];
                if (v.y >= 1)
                    newUVs[i] = newMesh.uv[(int)v.y - 1];

                if (v.z >= 1)
                    newNormals[i] = newMesh.normals[(int)v.z - 1];
                i++;
            }

            Mesh mesh = new Mesh();

            mesh.vertices = newVerts;
            mesh.uv = newUVs;
            mesh.normals = newNormals;
            mesh.triangles = newMesh.triangles;

            mesh.RecalculateBounds();
            mesh.Optimize();

            return mesh;
        }

        private static meshStruct createMeshStruct(string filename)
        {
            int triangles = 0;
            int vertices = 0;
            int vt = 0;
            int vn = 0;
            int face = 0;
            meshStruct mesh = new meshStruct();
            mesh.fileName = filename;
            StreamReader stream = File.OpenText(filename);
            string entireText = stream.ReadToEnd();
            stream.Close();
            using (StringReader reader = new StringReader(entireText))
            {
                string currentText = reader.ReadLine();
                char[] splitIdentifier = { ' ' };
                string[] brokenString;
                while (currentText != null)
                {
                    if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ")
                        && !currentText.StartsWith("vn "))
                    {
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");
                        }
                    }
                    else
                    {
                        currentText = currentText.Trim();                           //Trim the current line
                        brokenString = currentText.Split(splitIdentifier, 50);      //Split the line into an array, separating the original line by blank spaces
                        switch (brokenString[0])
                        {
                            case "v":
                                vertices++;
                                break;
                            case "vt":
                                vt++;
                                break;
                            case "vn":
                                vn++;
                                break;
                            case "f":
                                face = face + brokenString.Length - 1;
                                triangles = triangles + 3 * (brokenString.Length - 2); /*brokenString.Length is 3 or greater since a face must have at least
                                                                                     3 vertices.  For each additional vertice, there is an additional
                                                                                     triangle in the mesh (hence this formula).*/
                                break;
                        }
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");
                        }
                    }
                }
            }
            mesh.triangles = new int[triangles];
            mesh.vertices = new Vector3[vertices];
            mesh.uv = new Vector2[vt];
            mesh.normals = new Vector3[vn];
            mesh.faceData = new Vector3[face];
            return mesh;
        }

        private static void populateMeshStruct(ref meshStruct mesh)
        {
            StreamReader stream = File.OpenText(mesh.fileName);
            string entireText = stream.ReadToEnd();
            stream.Close();
            using (StringReader reader = new StringReader(entireText))
            {
                string currentText = reader.ReadLine();

                char[] splitIdentifier = { ' ' };
                char[] splitIdentifier2 = { '/' };
                string[] brokenString;
                string[] brokenBrokenString;
                int f = 0;
                int f2 = 0;
                int v = 0;
                int vn = 0;
                int vt = 0;
                int vt1 = 0;
                int vt2 = 0;
                while (currentText != null)
                {
                    if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ") &&
                        !currentText.StartsWith("vn ") && !currentText.StartsWith("g ") && !currentText.StartsWith("usemtl ") &&
                        !currentText.StartsWith("mtllib ") && !currentText.StartsWith("vt1 ") && !currentText.StartsWith("vt2 ") &&
                        !currentText.StartsWith("vc ") && !currentText.StartsWith("usemap "))
                    {
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");
                        }
                    }
                    else
                    {
                        currentText = currentText.Trim();
                        brokenString = currentText.Split(splitIdentifier, 50);
                        switch (brokenString[0])
                        {
                            case "g":
                                break;
                            case "usemtl":
                                break;
                            case "usemap":
                                break;
                            case "mtllib":
                                break;
                            case "v":
                                mesh.vertices[v] = new Vector3(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]),
                                                         System.Convert.ToSingle(brokenString[3]));
                                v++;
                                break;
                            case "vt":
                                mesh.uv[vt] = new Vector2(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
                                vt++;
                                break;
                            case "vt1":
                                mesh.uv[vt1] = new Vector2(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
                                vt1++;
                                break;
                            case "vt2":
                                mesh.uv[vt2] = new Vector2(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
                                vt2++;
                                break;
                            case "vn":
                                mesh.normals[vn] = new Vector3(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]),
                                                        System.Convert.ToSingle(brokenString[3]));
                                vn++;
                                break;
                            case "vc":
                                break;
                            case "f":

                                int j = 1;
                                List<int> intArray = new List<int>();
                                while (j < brokenString.Length && ("" + brokenString[j]).Length > 0)
                                {
                                    Vector3 temp = new Vector3();
                                    brokenBrokenString = brokenString[j].Split(splitIdentifier2, 3);    //Separate the face into individual components (vert, uv, normal)
                                    temp.x = System.Convert.ToInt32(brokenBrokenString[0]);
                                    if (brokenBrokenString.Length > 1)                                  //Some .obj files skip UV and normal
                                    {
                                        if (brokenBrokenString[1] != "")                                    //Some .obj files skip the uv and not the normal
                                        {
                                            temp.y = System.Convert.ToInt32(brokenBrokenString[1]);
                                        }
                                        temp.z = System.Convert.ToInt32(brokenBrokenString[2]);
                                    }
                                    j++;

                                    mesh.faceData[f2] = temp;
                                    intArray.Add(f2);
                                    f2++;
                                }
                                j = 1;
                                while (j + 2 < brokenString.Length)     //Create triangles out of the face data.  There will generally be more than 1 triangle per face.
                                {
                                    mesh.triangles[f] = intArray[0];
                                    f++;
                                    mesh.triangles[f] = intArray[j];
                                    f++;
                                    mesh.triangles[f] = intArray[j + 1];
                                    f++;

                                    j++;
                                }
                                break;
                        }
                        currentText = reader.ReadLine();
                        if (currentText != null)
                        {
                            currentText = currentText.Replace("  ", " ");       //Some .obj files insert double spaces, this removes them.
                        }
                    }
                }
            }
        }
    }
}
