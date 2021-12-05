import * as createBallScene from "./js/ball.js";
const canvas = document.getElementById("canvas"); // Get the canvas element
const engine = new BABYLON.Engine(canvas, true); // Generate the BABYLON 3D engine

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

const scene = createBallScene();
// Register a render loop to repeatedly render the scene
engine.runRenderLoop(function () {
	scene.render();
});

// Watch for browser/canvas resize events
window.addEventListener("resize", function () {
	engine.resize();
});