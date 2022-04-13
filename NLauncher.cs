using Godot;
using System;

public class NLauncher : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    string dVersion = "none";


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //https://api.github.com/repos/thekaigonzalez/NFyMono/git/refs/tags
        GetNode("MonoAllV").Connect("request_completed", this, "OnGetAllVersions");
		HTTPRequest httpRequest = GetNode<HTTPRequest>("MonoAllV");
		httpRequest.Request("https://api.github.com/repos/thekaigonzalez/NFyMono/releases"); // request latest release

    }

    public void OnGetAllVersions(int result, int response_code, string[] headers, byte[] body) {
        string s = System.Text.Encoding.UTF8.GetString(body);
		if (response_code == 200) {
			var js1 =  JSON.Parse(s).Result;
			var js =  (Godot.Collections.Array) js1;

            foreach (var entry in js) {
                var edict = entry as Godot.Collections.Dictionary;
                GetNode<OptionButton>("NMonoScreen/VersionList").AddItem(edict["tag_name"] as string);

            }
        }
    }

    public void _on_DV_pressed() {
        string ver = GetNode<OptionButton>("NMonoScreen/VersionList").GetItemText(GetNode<OptionButton>("NMonoScreen/VersionList").GetSelectedId());
		HTTPRequest httpRequest = GetNode<HTTPRequest>("MonoVerSelect");
        httpRequest.DownloadFile =  "nmono" + ver + ".exe";
        string ver1 = ver.Substring(ver.IndexOf(".")+1).Trim(); // (version.)NUMBER
        /* if installer available */
        if (ver1 != "version.5.macos.beta") {
            if (ver1.ToInt() < 5) {
                httpRequest.Request("https://github.com/thekaigonzalez/NFyMono/releases/download/" + ver + "/NFyMonoSetup.exe");
                OS.ShellOpen("nmono" + ver + ".exe");
            }
        }

    }


    public void _on_DownloadLST_pressed() {
        GetNode("MonoLTSHTTP").Connect("request_completed", this, "OnVersionRequestCompleted");
		HTTPRequest httpRequest = GetNode<HTTPRequest>("MonoLTSHTTP");
		httpRequest.Request("https://api.github.com/repos/thekaigonzalez/NFyMono/releases/latest"); // request latest release

    }

    public void OnVersionRequestCompleted(int result, int response_code, string[] headers, byte[] body)
	{
		string s = System.Text.Encoding.UTF8.GetString(body);
		if (response_code == 200) {
			var js1 =  JSON.Parse(s).Result;
			var js =  (Godot.Collections.Dictionary) js1;
            var tag = js["tag_name"] as string;

		    HTTPRequest httpRequest = GetNode<HTTPRequest>("MonoDownloadV");

            httpRequest.DownloadFile =  "nmono" + tag + ".exe";
		    httpRequest.Request("https://github.com/thekaigonzalez/NFyMono/releases/download/" + tag + "/NFyMonoSetup.exe"); // request latest release
            OS.ShellOpen("nmono" + tag + ".exe");
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
