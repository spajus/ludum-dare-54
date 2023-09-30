using Godot;
using System;

public partial class PlayerController : CharacterBody3D {
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private float shootDistance = 1000f;

    //private Node3D nozzle;
    private Camera3D camera;
    //private RayCast3D gunRay;
    [Export]
    private float bulletSpeed = 1000f;

    public override void _Ready() {
        camera = GetNode<Camera3D>(Nodes.Camera);
    }

    public override void _PhysicsProcess(double delta) {
        Velocity = HandleInput(Velocity, (float) delta);
        MoveAndSlide();
    }

    private Vector3 HandleInput(Vector3 velocity, float delta) {
        if (!Runtime.IsFocused) { return velocity; }
        // Add the gravity.
        if (!IsOnFloor()) {
            velocity.Y -= gravity * delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("jump") && IsOnFloor()) {
            velocity.Y = JumpVelocity;
        }

        if (Input.IsActionJustPressed("shoot")) {
            FireBullet();
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
        Vector3 direction = (Transform.Basis *
            new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero) {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        } else {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }
        return velocity;
    }
    private void FireBullet() {
        var nozzle = GetNode<Node3D>(Nodes.Nozzle);
        var gunRay = GetNode<RayCast3D>(Nodes.GunRay);
        var bullet = Runtime.BulletPool.Spawn();
        var gunDir = gunRay.ToGlobal(gunRay.TargetPosition);
        GD.Print("Ray: {0}:{1}", gunRay, gunDir);
        var gunPos = gunRay.GlobalPosition;
        var bulletPos = gunPos + gunDir * 1f;
        var pointInDistance = gunPos + gunDir * 1000;
        //var bulletOrigin = nozzle.GlobalTransform.Origin;
        //var bulletDir = pointInDistance - bulletOrigin;
        Runtime.Root.AddChild(bullet);
        bullet.GlobalPosition = gunPos;
        bullet.GlobalRotation = gunRay.GlobalRotation;
        //bullet.GlobalPosition = bulletOrigin;
        bullet.LinearVelocity = gunDir.Normalized() * bulletSpeed;
    }

    private float mouseSensitivity = 0.1f;

    public override void _Input(InputEvent evt) {
        if (!Runtime.IsFocused) { return; }
        if (evt is InputEventMouseMotion eventMouseMotion) {
            // Rotate the player left and right
            RotationDegrees = new Vector3(
                RotationDegrees.X,
                RotationDegrees.Y - eventMouseMotion.Relative.X * mouseSensitivity,
                RotationDegrees.Z
            );

            // Access the Camera3D node to rotate it up and down
            camera.RotationDegrees = new Vector3(
                Mathf.Clamp(camera.RotationDegrees.X -
                    eventMouseMotion.Relative.Y * mouseSensitivity, -80, 80),
                camera.RotationDegrees.Y,
                camera.RotationDegrees.Z
            );
        }
    }
}
