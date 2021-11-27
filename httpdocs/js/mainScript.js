window.addEventListener("DOMContentLoaded", function () {
	var canvas = document.getElementById("canvas");
	var engine = new BABYLON.Engine(canvas, true);

	var createScene = function () {
		const scene = new BABYLON.Scene(engine);
		const camera = new BABYLON.ArcRotateCamera("camera",
			-Math.PI / 2, Math.PI / 2.5, 3,
			new BABYLON.Vector3(0, 0, 0));
		camera.attachControl(canvas, true);
		const light = new BABYLON.HemisphericLight("light", new BABYLON.Vector3(0, 1, 0));

		const box = BABYLON.MeshBuilder.CreateBox("box", {});

		return scene;
	}
	var scene = createScene();
	engine.runRenderLoop(function () {
		scene.render();
	});
});