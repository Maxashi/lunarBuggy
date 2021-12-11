export default class ballScene {
	constructor() {
	}
}

export function createBallScene(engine) {
	// delete engine;
	engine = new BABYLON.Engine(canvas, true, {
		deterministicLockstep: false,
		lockstepMaxSteps: 1
	});

	// This creates a basic Babylon Scene object (non-mesh)
	var scene = new BABYLON.Scene(engine);

	var physEngine = new BABYLON.AmmoJSPlugin(false);
	scene.enablePhysics(new BABYLON.Vector3(0, -9.8, 0), physEngine);
	physEngine.setTimeStep(1 / 60);

	// This creates and positions a free camera (non-mesh)
	var camera = new BABYLON.FreeCamera("camera1", new BABYLON.Vector3(0, 5, -10), scene);

	// This targets the camera to scene origin
	camera.setTarget(BABYLON.Vector3.Zero());

	// This attaches the camera to the canvas
	camera.attachControl(canvas, true);

	// This creates a light, aiming 0,1,0 - to the sky (non-mesh)
	var light = new BABYLON.HemisphericLight("light1", new BABYLON.Vector3(0, 1, 0), scene);

	// Default intensity is 1. Let's dim the light a small amount
	light.intensity = 0.7;

	// Our built-in 'sphere' shape. Params: name, subdivs, size, scene
	var sphere = BABYLON.Mesh.CreateSphere("sphere1", 16, 2, scene);
	sphere.position.y = 4;

	// Our built-in 'ground' shape. Params: name, width, depth, subdivs, scene
	var ground = BABYLON.Mesh.CreateGround("ground1", 15, 10, 2, scene);

	var box1 = BABYLON.Mesh.CreateBox("Box1", 2.0, scene);
	box1.position.x = -3;
	box1.scaling.y = 0.5;
	box1.scaling.z = 0.3;
	var mat = new BABYLON.StandardMaterial('boxMat', scene);
	mat.emissiveColor = new BABYLON.Color3(0, 0, 1);
	box1.material = mat;

	sphere.physicsImpostor = new BABYLON.PhysicsImpostor(sphere, BABYLON.PhysicsImpostor.SphereImpostor, { mass: 8, restitution: 0.9 }, scene);
	ground.physicsImpostor = new BABYLON.PhysicsImpostor(ground, BABYLON.PhysicsImpostor.BoxImpostor, { mass: 0, restitution: 0.9 }, scene);

	scene.onBeforeStepObservable.add(function (theScene) {
		//console.log("Performing game logic, BEFORE animations and physics for stepId: "+theScene.getStepId());
		box1.rotation.y += 0.05;
	});

	scene.onAfterStepObservable.add(function (theScene) {
		//console.log("Performing game logic, AFTER animations and physics for stepId: "+theScene.getStepId());
		if (sphere.physicsImpostor.getLinearVelocity().length() < BABYLON.PhysicsEngine.Epsilon) {
			console.log("sphere is at rest on stepId: " + theScene.getStepId());
			console.log("box1.rotation.y is: " + box1.rotation.y);
			theScene.onAfterStepObservable.clear();
			theScene.onBeforeStepObservable.clear();
		}
	});

	return scene;
};

