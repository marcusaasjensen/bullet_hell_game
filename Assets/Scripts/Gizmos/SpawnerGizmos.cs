using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpawnerController))]
public class SpawnerGizmos : MonoBehaviour
{
    [SerializeField] SpawnerController _spawnerController;

    [Header("Direction visualizer")]
    [SerializeField] Color _lineGizmosColor = Color.white;

    [Header("Scopes visualizer")]
    [SerializeField] float _scopeCubeGizmosSize = .1f;
    [SerializeField] Color _scopeCubeGizmosColor = Color.black;

    void OnDrawGizmos()
    {
        if (!_spawnerController)
        {
            Debug.LogWarning("Spawner Controller reference in Spawner Gizmos script is missing.", this);
            return; 
        }

        SpawningBehaviour behaviour = _spawnerController.SpawningBehaviour;
        
        if (!behaviour)
        {
            Debug.LogWarning("Spawning Behaviour reference in spawner controller is missing.", this);
            return;
        }

        ProjectileController proj = _spawnerController.SpawningBehaviour.ProjectileToSpawnConfig;

        if(!proj)
        {
            Debug.LogWarning("Projectile Controller reference (config) in spawner controller is missing.", this);
            return;
        }

        Vector3 gizmosLineEndPos;
        Vector3 endPosition;

        float angle;
        float initAngle = 0;
        float offSet = 360 / _spawnerController.SpawningBehaviour.NumberOfScopes;
        float nBProjectilePerScope = _spawnerController.SpawningBehaviour.NumberOfProjectiles / behaviour.NumberOfScopes;
        float counter = 0;

        float rayLength;

        float rotationOffset = behaviour.SpawnerDirectionAffectsProjectile ? _spawnerController.ProjectileMovement.Direction : 0;

        for (int j = 0; j < behaviour.NumberOfScopes; j++)
        {
            angle = initAngle;
            endPosition = CalculateEndPoint(2 * proj.ProjectileMovement.Direction, angle, rotationOffset);
            Gizmos.color = _scopeCubeGizmosColor;
            Gizmos.DrawCube(_spawnerController.transform.position + endPosition, _scopeCubeGizmosSize * Vector3.one);

            for (int i = 1; i <= nBProjectilePerScope; i++)
            {
                rayLength = (proj.ProjectileMovement.MoveSpeed + GetNewMoveSpeed(proj.ProjectileMovement.MoveSpeed, counter)) * proj.TimeToLive;

                if (counter < behaviour.NumberOfProjectiles)
                    counter++;

                endPosition = CalculateEndPoint(2 * proj.ProjectileMovement.Direction, angle, rotationOffset);
                gizmosLineEndPos = rayLength * endPosition;

                Gizmos.color = _lineGizmosColor;
                Gizmos.DrawLine(_spawnerController.transform.position, _spawnerController.transform.position + gizmosLineEndPos);

                angle += offSet * behaviour.ScopeRange / nBProjectilePerScope;
                if (i == nBProjectilePerScope)
                {
                    Gizmos.color = _scopeCubeGizmosColor;
                    Gizmos.DrawCube(_spawnerController.transform.position + endPosition, _scopeCubeGizmosSize * Vector3.one);
                }
            }
            initAngle += offSet;
        }
    }

    Vector3 CalculateEndPoint(float directionInDegree, float angle, float offset)
    {
        float xPosition = Mathf.Cos(GetNewDirection(directionInDegree, angle, offset) * Mathf.Deg2Rad);
        float yPosition = Mathf.Sin(GetNewDirection(directionInDegree, angle, offset) * Mathf.Deg2Rad);
        return new(xPosition, yPosition, 0);
    }

    float GetNewDirection(float dir, float angle, float offSet)
    {
        SpawningBehaviour behaviour = _spawnerController.SpawningBehaviour;

        return dir + offSet + angle * behaviour.SpawningRange;
    }

    float GetNewMoveSpeed(float speed, float step)
    {
        SpawningBehaviour behaviour = _spawnerController.SpawningBehaviour;

        return speed + Mathf.Pow(Mathf.Sin(behaviour.NumberOfSides * Mathf.PI / behaviour.NumberOfProjectiles * step), 2) * behaviour.SideBending;
    }
}
