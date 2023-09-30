using Godot;
using System;

public partial class PlayerController : CharacterBody3D {
    public const float Speed = 2.0f;
    public const float JumpVelocity = 3.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private float shootDistance = 1000f;

    private Camera3D camera;
    private RayCast3D sightRay;
    private CanvasModulate crosshair;
    private float dropSpeed = 2;
    private float bookForceSpeed = 2f;
    private float bookCarryMass = 0.05f;
    private float rotateSpeed = 0.05f;
    private float holdDistance = 1f;
    private float minHoldDist = 0.25f;
    private float maxHoldDist = 2f;
    private float distChangeSpeed = 0.25f;

    private Book carriedBook;
    private bool isRotMode;

    public override void _Ready() {
        camera = GetNode<Camera3D>(Nodes.Camera);
        sightRay = GetNode<RayCast3D>(Nodes.SightRay);
        crosshair = GetNode<CanvasModulate>(Nodes.CrosshairModulate);
    }

    public override void _PhysicsProcess(double delta) {
        isRotMode = Input.IsActionPressed("rot_mode");
        Velocity = HandleInput(Velocity, (float) delta);
        MoveAndSlide();
        MoveCarriedBook();
        RaycastSight();
    }

    private void MoveCarriedBook() {
        if (carriedBook == null) { return; }
        var sightDir = sightRay.ToGlobal(sightRay.TargetPosition);
        var rayPos = sightRay.GlobalPosition;

        var wantedPos = rayPos + sightDir.Normalized() * holdDistance;

        Vector3 distanceToTarget = wantedPos - carriedBook.GlobalTransform.Origin;
        Vector3 force = Vector3.Zero;
        if (distanceToTarget.Length() > 0.001f)  {
            Vector3 direction = distanceToTarget;
            var len = distanceToTarget.Length();
            direction = direction.Normalized() * (len * len);
            force = direction * bookForceSpeed;
        }

        if (mouseRot.LengthSquared() > 0.001f) {
            carriedBook.ApplyTorque(mouseRot * rotateSpeed);
            mouseRot = Vector3.Zero;
        }
        carriedBook.ApplyForce(force);
    }

    private void RaycastSight() {
        if (sightRay.IsColliding()) {
            crosshair.Color = Colors.Green;
        } else {
            crosshair.Color = Colors.White;
        }
    }

    private Vector3 HandleInput(Vector3 velocity, float delta) {

        if (!Runtime.IsFocused || (isRotMode && carriedBook != null)) {
            return velocity;
        }
        // Add the gravity.
        if (!IsOnFloor()) {
            velocity.Y -= gravity * delta;
        }

        var isOnFloor = IsOnFloor();

        // Handle Jump.
        if (isOnFloor && Input.IsActionJustPressed("jump")) {
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
        if (isOnFloor) {
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
        book.Set("mass", bookCarryMass);
        book.Set("linear_damp", 5f);
        book.Set("angular_damp", 5f);
        carriedBook = book;
    }

    private void DropBook() {
        carriedBook.Set("mass", 1f);
        carriedBook.Set("linear_damp", 0f);
        carriedBook.Set("angular_damp", 0f);
        carriedBook = null;
    }

    private void SpawnBook() {
        var book = Runtime.BookPool.Spawn();
        var sightDir = sightRay.ToGlobal(sightRay.TargetPosition);
        var rayPos = sightRay.GlobalPosition;
        Runtime.Root.AddChild(book);
        book.GlobalPosition = rayPos;
        var rand = new Random();
        book.GlobalRotation = new Vector3(
                rand.NextSingle() * 180f,
                rand.NextSingle() * 180f,
                rand.NextSingle() * 180f);
        book.LinearVelocity = sightDir.Normalized() * dropSpeed;
    }

    private float mouseSensitivity = 0.1f;

    private Vector3 mouseRot;


    public override void _Input(InputEvent evt) {
        if (!Runtime.IsFocused) { return; }

        if (evt is InputEventMouseButton emb) {
            if (emb.IsPressed()) {
                if (carriedBook != null) {
                    var dir = 0f;
                    if (emb.ButtonIndex == MouseButton.WheelUp) {
                        dir = distChangeSpeed;
                    }
                    if (emb.ButtonIndex == MouseButton.WheelDown) {
                        dir = -distChangeSpeed;
                    }
                    if (dir != 0f) {
                        holdDistance = Mathf.Clamp(holdDistance + dir,
                            minHoldDist, maxHoldDist);
                    }
                }
            }
        }
        if (evt is InputEventMouseMotion eventMouseMotion) {
            if (isRotMode) {
                mouseRot = new Vector3(
                    eventMouseMotion.Relative.Y * mouseSensitivity,
                    eventMouseMotion.Relative.X * mouseSensitivity,
                    0f);
                return;
            }
            mouseRot = Vector3.Zero;
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
