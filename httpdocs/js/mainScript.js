import ballScene, { createBallScene } from "/js/ball.js";

const canvas = document.getElementById("canvas"); // Get the canvas element
const engine = new BABYLON.Engine(canvas, true); // Generate the BABYLON 3D engine

// Register a render loop to repeatedly render the scene
const scene = createBallScene(engine);

engine.runRenderLoop(function () {
	scene.render();
});

// Watch for browser/canvas resize events
window.addEventListener("resize", function () {
	engine.resize();
});