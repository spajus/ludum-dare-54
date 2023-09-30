using Godot;
using System;

public partial class PlayerController : CharacterBody3D {
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private float shootDistance = 1000f;

    private Camera3D camera;
    private RayCast3D sightRay;
    private CanvasModulate crosshair;
    [Export]
    private float dropSpeed = 2;

    private Book carriedBook;

    public override void _Ready() {
        camera = GetNode<Camera3D>(Nodes.Camera);
        sightRay = GetNode<RayCast3D>(Nodes.SightRay);
        crosshair = GetNode<CanvasModulate>(Nodes.CrosshairModulate);
    }

    public override void _PhysicsProcess(double delta) {
        Velocity = HandleInput(Velocity, (float) delta);
        MoveAndSlide();
        MoveCarriedBook();
        RaycastSight();
    }

    private void MoveCarriedBook() {
        if (carriedBook == null) { return; }
        var sightDir = sightRay.ToGlobal(sightRay.TargetPosition);
        var rayPos = sightRay.GlobalPosition;
        carriedBook.GlobalPosition = rayPos + sightDir.Normalized() * 1f;
    }

    private void RaycastSight() {
        if (sightRay.IsColliding()) {
            crosshair.Color = Colors.Green;
        } else {
            crosshair.Color = Colors.White;
        }
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

        if (Input.IsActionJustPressed("spawn_book")) {
            SpawnBook();
        }
        if (Input.IsActionJustPressed("interact")) {
            Interact();
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
    private void Interact() {
        if (carriedBook != null) {
            DropBook();
            return;
        }
        if (!sightRay.IsColliding()) { return; }
        var bookCol = (Node3D) sightRay.GetCollider();
        var book = bookCol.GetNode<Book>(bookCol.GetPath());
        CarryBook(book);
        GD.Print(book);
    }

    private void CarryBook(Book book) {
        book.Set("mass", 0f);
        carriedBook = book;
    }

    private void DropBook() {
        carriedBook.Set("mass", 1f);
        carriedBook = null;
    }

    private void SpawnBook() {
        var book = Runtime.BookPool.Spawn();
        var sightDir = sightRay.ToGlobal(sightRay.TargetPosition);
        var rayPos = sightRay.GlobalPosition;
        Runtime.Root.AddChild(book);
        book.GlobalPosition = rayPos;
        book.GlobalRotation = sightRay.GlobalRotation;
        book.LinearVelocity = sightDir.Normalized() * dropSpeed;
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
