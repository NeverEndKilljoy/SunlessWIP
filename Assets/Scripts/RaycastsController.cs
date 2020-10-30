using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastsController : MonoBehaviour
{
    public int collisionMask = (1 << 9) | (1 << 10); // Por defecto hacemos colisión con suelo y enemigo
    int playerLayer = 8;
    int groundLayer = 9;
    int enemyLayer = 10;

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    new BoxCollider2D collider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void ChangeCollisions( int dashing)
    {
        if (dashing == 1)
        {
            collisionMask = 1 << groundLayer;
            //Debug.Log(Convert.ToString(collisionMask, 2).PadLeft(32, '0'));
        } else
        {
            collisionMask = (1 << groundLayer) | (1 << enemyLayer);
            //Debug.Log(Convert.ToString(collisionMask, 2).PadLeft(32, '0'));
        }
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (velocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        transform.Translate(velocity);

    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; //if para ver si los raycast deben ser dibujados izquierda o derecha.

            rayOrigin += Vector2.up * (horizontalRaySpacing * i); //Dibuja i cantidad de rayos
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance; //si un rayo pega con algo, y otro rayo pega con otro objeto debajo, no salta el primer objeto y el segundo rayo no pega al objeto debajo.

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft; //if para ver si los raycast deben ser dibujados arriba o abajo.
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x); //Dibuja i cantidad de rayos
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red); //Dibuja cada raycast vertical desde bottomLeft

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance; //si un rayo pega con algo, y otro rayo pega con otro objeto debajo, no salta el primer objeto y el segundo rayo no pega al objeto debajo.

                collisions.below = directionY == -1; //Evita que se acumule gravedad estando en el suelo
                collisions.above = directionY == 1;
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2); //shrinks both sizes by skinWidth

        //Raycasts origin points:
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2); //shrinks both sides by skinWidth

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1); //Espacio entre raycasts
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }



    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
