using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;
using GroupProject.Opening;

public static class OpeningSceneBuilder
{
    const string Root = "Assets/GroupProject/Scene01_Opening_Adham";
    const string Pack = "Assets/Modular_SciFi_Pack (146 Objects, LowPoly)/Prefabs/Cleen/";
    static Material cyan, dark, white;
    [MenuItem("Tools/Adham/Build Opening Scene")]
    public static void Build()
    {
        var path = Root + "/Scenes/Scene01_Opening_Adham.unity";
        if (File.Exists(path)) { Debug.LogWarning("Opening scene already exists; preserving your edits."); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.skybox = null;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.075f,.095f,.13f);
        RenderSettings.reflectionIntensity = .2f;
        cyan = Mat("Guidance_Cyan", new Color(.08f,.78f,.88f), true);
        white = Mat("Soft_White", new Color(.7f,.87f,.9f), true);
        dark = Mat("Panel_Dark", new Color(.018f,.033f,.05f), false);
        var root = new GameObject("Scene01_Opening_Adham").transform;
        var env = Group("Environment", root);
        var lights = Group("Lighting", root);
        var guidance = Group("FloorGuidance", root);
        var ui = Group("InstructionUI", root);
        var audio = Group("WelcomeAudio", root);
        var elevator = Group("ElevatorEntrance", root);
        for (int i=0; i<4; i++)
        {
            float z = i*4;
            Fit("Floor/Floor4_4.prefab", env, new Vector3(0,-.12f,z), new Vector3(4,.24f,4));
            Fit("Ceiling/Ceiling4_4.prefab", env,new Vector3(0,3.35f,z),new Vector3(4,.2f,4));
            Fit("Wall1/Wall2_3.prefab",env,new Vector3(-2.08f,1.6f,z),new Vector3(.2f,3.2f,4),90);
            Fit("Wall1/Wall2_3.prefab",env,new Vector3(2.08f,1.6f,z),new Vector3(.2f,3.2f,4),-90);
            Box("Ceiling light",lights,new Vector3(0,3.19f,z),new Vector3(.65f,.035f,1.4f),white,false);
            var light = new GameObject("Central pool " + (i+1)).AddComponent<Light>();
            light.transform.SetParent(lights,false); light.transform.localPosition=new Vector3(0,3.05f,z);
            light.transform.localRotation=Quaternion.Euler(90,0,0);
            light.type=LightType.Spot; light.spotAngle=76; light.range=7; light.intensity=10f;
            light.color=new Color(.68f,.83f,1); light.shadows=LightShadows.None;
        }
        Box("Rear bulkhead",env,new Vector3(0,1.6f,-2),new Vector3(4,3.2f,.2f),dark,true);
        for (int i=0;i<23;i++)
            Box("Path segment " + i,guidance,new Vector3(0,.025f,-.4f+i*.55f),new Vector3(.065f,.018f,.38f),cyan,false);
        for(int i=0;i<4;i++) {
            float z=2+i*2.7f;
            var a=Box("Arrow left",guidance,new Vector3(-.13f,.03f,z),new Vector3(.035f,.02f,.38f),cyan,false);
            a.transform.localRotation=Quaternion.Euler(0,45,0);
            var b=Box("Arrow right",guidance,new Vector3(.13f,.03f,z),new Vector3(.035f,.02f,.38f),cyan,false);
            b.transform.localRotation=Quaternion.Euler(0,-45,0);
        }
        Fit("Door/Door1.prefab",elevator,new Vector3(0,1.45f,13.85f),new Vector3(2.5f,2.9f,.32f));
        Box("Left door surround",elevator,new Vector3(-1.7f,1.6f,13.85f),new Vector3(.65f,3.2f,.4f),dark,true);
        Box("Right door surround",elevator,new Vector3(1.7f,1.6f,13.85f),new Vector3(.65f,3.2f,.4f),dark,true);
        Box("Header",elevator,new Vector3(0,3.05f,13.8f),new Vector3(3,.3f,.4f),dark,true);
        Text("Elevator label",elevator,"E L E V A T O R   /   0 1",new Vector3(0,2.98f,13.55f),Quaternion.identity,.07f,white.color);
        Box("Door indicator",elevator,new Vector3(0,2.72f,13.6f),new Vector3(.8f,.025f,.03f),cyan,false);
        Box("Call panel - future interaction",elevator,new Vector3(1.57f,1.2f,13.57f),new Vector3(.2f,.35f,.06f),dark,true);
        Box("Call indicator",elevator,new Vector3(1.57f,1.22f,13.53f),new Vector3(.08f,.08f,.02f),cyan,false);
        var panel=Group("Movement instructions - left wall",ui);
        panel.localPosition=new Vector3(-1.88f,1.65f,.8f); panel.localRotation=Quaternion.Euler(0,-65,0);
        Box("Panel",panel,Vector3.zero,new Vector3(1.65f,1.08f,.04f),dark,false);
        Box("Accent",panel,new Vector3(-.76f,0,-.03f),new Vector3(.018f,.91f,.01f),cyan,false);
        Text("Heading",panel,"WELCOME ABOARD",new Vector3(0,.36f,-.035f),Quaternion.identity,.09f,cyan.color);
        Text("Controls",panel,"LEFT STICK   Move\nRIGHT STICK   Snap turn\nTRIGGER   Select / interact\n\nFollow the floor lights to the elevator.",new Vector3(0,-.04f,-.035f),Quaternion.identity,.06f,Color.white);
        Text("Forward prompt",ui,"FOLLOW THE LIGHT",new Vector3(0,2.6f,6),Quaternion.identity,.07f,cyan.color);
        var source=new GameObject("Overhead welcome - prototype voice").AddComponent<AudioSource>();
        source.transform.SetParent(audio,false); source.transform.localPosition=new Vector3(0,2.8f,1);
        source.clip=AssetDatabase.LoadAssetAtPath<AudioClip>(Root+"/Audio/Welcome_Prototype.wav");
        source.playOnAwake=false; source.spatialBlend=1; source.rolloffMode=AudioRolloffMode.Linear;
        source.minDistance=3; source.maxDistance=20; source.pitch=.88f; source.volume=.85f;
        Text("Welcome transcript",ui,"Welcome. Take a moment to look around.\nYour controls are displayed to your left.\nWhen ready, follow the path towards the elevator.",new Vector3(1.87f,1.65f,1),Quaternion.Euler(0,65,0),.055f,new Color(.65f,.77f,.8f));
        var rig=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Samples/XR Interaction Toolkit/3.5.1/Starter Assets/Prefabs/XR Origin (XR Rig).prefab"));
        rig.name="Shared_XR_Player"; rig.transform.position=new Vector3(0,0,-.8f);
        var camera=rig.GetComponentInChildren<Camera>(); camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=Color.black; camera.nearClipPlane=.05f; camera.farClipPlane=60;
        foreach(var c in rig.GetComponentsInChildren<MonoBehaviour>(true)) {
            if(!c) continue;
            var so=new SerializedObject(c); var speed=so.FindProperty("m_MoveSpeed"); if(speed!=null){speed.floatValue=1.4f;so.ApplyModifiedPropertiesWithoutUndo();}
        }
        new GameObject("Shared_XR_Interaction_Manager").AddComponent<XRInteractionManager>();
        var seq=root.gameObject.AddComponent<OpeningSequence>(); seq.headsetCamera=camera; seq.welcome=source;
        seq.fadeMaterial=new Material(Shader.Find("GroupProject/OpeningFade")); AssetDatabase.CreateAsset(seq.fadeMaterial,Root+"/Materials/OpeningFade.mat");
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),path);
        Selection.activeGameObject=root.gameObject;
        Tune();
        Preview();
        Debug.Log("OPENING BUILD PASS: scene saved; imported modular meshes, single XR camera, voice and fade assigned.");
    }
    static Transform Group(string name,Transform parent) { var t=new GameObject(name).transform;t.SetParent(parent,false);return t; }
    static Material Mat(string name,Color color,bool unlit) {
        var m=new Material(Shader.Find(unlit?"Universal Render Pipeline/Unlit":"Universal Render Pipeline/Lit"));m.color=color;AssetDatabase.CreateAsset(m,Root+"/Materials/"+name+".mat");return m;
    }
    static GameObject Box(string name,Transform parent,Vector3 pos,Vector3 size,Material mat,bool collider) {
        var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;if(!collider)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;
    }
    static void Fit(string asset,Transform parent,Vector3 center,Vector3 size,float yaw=0) {
        var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Pack+asset);if(!prefab)throw new Exception("Missing asset "+asset);
        var g=(GameObject)PrefabUtility.InstantiatePrefab(prefab);g.transform.SetParent(parent,false);g.transform.localRotation=Quaternion.Euler(0,yaw,0);
        var wrap=Group(g.name+" module",parent);g.transform.SetParent(wrap,true);
        var rs=g.GetComponentsInChildren<Renderer>();var bounds=rs[0].bounds;foreach(var r in rs)bounds.Encapsulate(r.bounds);
        g.transform.position-=bounds.center;
        wrap.localScale=new Vector3(size.x/Mathf.Max(bounds.size.x,.001f),size.y/Mathf.Max(bounds.size.y,.001f),size.z/Mathf.Max(bounds.size.z,.001f));wrap.localPosition=center;
        var col=wrap.gameObject.AddComponent<BoxCollider>();col.size=bounds.size;
    }
    static void Text(string name,Transform parent,string value,Vector3 pos,Quaternion rot,float size,Color color) {
        var g=new GameObject(name);g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localRotation=rot;
        var t=g.AddComponent<TextMesh>();t.text=value;t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=64;t.characterSize=size;t.anchor=TextAnchor.MiddleCenter;t.alignment=TextAlignment.Center;t.color=color;g.GetComponent<MeshRenderer>().sharedMaterial=t.font.material;
    }
    [MenuItem("Tools/Adham/Tune Opening Presentation")]
    public static void Tune() {
        var root=GameObject.Find("Scene01_Opening_Adham");
        if(!root)return;
        root.transform.Find("InstructionUI/Movement instructions - left wall").localRotation=Quaternion.Euler(0,-65,0);
        root.transform.Find("InstructionUI/Welcome transcript").localRotation=Quaternion.Euler(0,65,0);
        foreach(var t in root.GetComponentsInChildren<TextMesh>()) {
            t.transform.localScale=Vector3.one;
            float w=1.4f,h=.2f;
            if(t.name=="Controls"){w=1.4f;h=.58f;}
            if(t.name=="Welcome transcript"){w=1.6f;h=.7f;}
            if(t.name=="Elevator label"){w=2;h=.15f;}
            if(t.name=="Forward prompt"){w=1.5f;h=.15f;}
            // TextMesh geometry is generated by the renderer, so measure world bounds in identity rotation.
            var rot=t.transform.rotation;t.transform.rotation=Quaternion.identity;
            var size=t.GetComponent<Renderer>().bounds.size;
            t.transform.rotation=rot;
            t.transform.localScale=Vector3.one*Mathf.Min(w/Mathf.Max(size.x,.001f),h/Mathf.Max(size.y,.001f));
        }
        foreach(var l in root.GetComponentsInChildren<Light>()) l.intensity=10;
        if(!root.transform.Find("Lighting/Downward fill")) {
            var fill=new GameObject("Downward fill").AddComponent<Light>();
            fill.transform.SetParent(root.transform.Find("Lighting"),false);
            fill.type=LightType.Directional;fill.transform.localRotation=Quaternion.Euler(90,0,0);
            fill.intensity=.8f;fill.color=new Color(.65f,.8f,1);fill.shadows=LightShadows.None;
        }
        var downward=root.transform.Find("Lighting/Downward fill").GetComponent<Light>();
        downward.transform.localRotation=Quaternion.Euler(60,0,0);downward.intensity=.45f;
        EditorSceneManager.MarkSceneDirty(root.scene);EditorSceneManager.SaveScene(root.scene);
    }
    [MenuItem("Tools/Adham/Capture Opening Preview")]
    public static void Preview() {
        var g=new GameObject("Temporary preview");var c=g.AddComponent<Camera>();c.transform.position=new Vector3(0,1.65f,-.8f);c.fieldOfView=75;c.clearFlags=CameraClearFlags.SolidColor;c.backgroundColor=Color.black;
        var rt=new RenderTexture(1280,800,24);c.targetTexture=rt;c.Render();var previous=RenderTexture.active;RenderTexture.active=rt;
        var tex=new Texture2D(1280,800,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1280,800),0,0);tex.Apply();Directory.CreateDirectory("Temp/OpeningReview");File.WriteAllBytes("Temp/OpeningReview/Opening.png",tex.EncodeToPNG());
        c.transform.rotation=Quaternion.Euler(0,-60,0);c.Render();RenderTexture.active=rt;
        tex.ReadPixels(new Rect(0,0,1280,800),0,0);tex.Apply();File.WriteAllBytes("Temp/OpeningReview/Instructions.png",tex.EncodeToPNG());
        RenderTexture.active=previous;c.targetTexture=null;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(g);
    }
}
