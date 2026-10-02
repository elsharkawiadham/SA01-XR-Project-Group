using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using GroupProject.Opening;
[InitializeOnLoad]
public static class OpeningReviewBridge
{
    const string Request="Temp/OpeningReview/request.txt";
    static OpeningReviewBridge() { EditorApplication.update += Tick; }
    static void Tick() {
        if(EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(Request)) return;
        string command=File.ReadAllText(Request).Trim();File.Delete(Request);
        try {
            if(command=="build") OpeningSceneBuilder.Build();
            if(command=="tune") { OpeningSceneBuilder.Tune(); OpeningSceneBuilder.Preview(); }
            if(command=="preview") OpeningSceneBuilder.Preview();
            if(command=="play") EditorApplication.isPlaying=true;
            if(command=="stop") EditorApplication.isPlaying=false;
            if(command=="validate") {
                var seq=Object.FindFirstObjectByType<OpeningSequence>();
                var cams=Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
                if(!seq || !seq.headsetCamera || !seq.welcome || !seq.welcome.clip || !seq.fadeMaterial || cams.Length!=1) throw new System.Exception("Missing sequence references or duplicate camera");
                foreach(var r in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None)) foreach(var m in r.sharedMaterials) if(m && !m.shader.isSupported) throw new System.Exception("Unsupported shader: "+m.name);
                if(EditorApplication.isPlaying && GameObject.Find("Opening fade (runtime)")) throw new System.Exception("Fade did not complete");
                File.WriteAllText("Temp/OpeningReview/validation.txt","PASS: sequence, audio, fade material, single XR camera, supported shaders; playing="+EditorApplication.isPlaying+"; camera="+seq.headsetCamera.transform.position+"; voicePlaying="+seq.welcome.isPlaying);
            }
        } catch(System.Exception ex) { Debug.LogException(ex);File.WriteAllText("Temp/OpeningReview/error.txt",ex.ToString()); }
    }
}
