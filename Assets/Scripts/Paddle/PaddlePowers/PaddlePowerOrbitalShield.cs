using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Paddle Powers/Orbital Shield")]
public class PaddlePowerOrbitalShield : PaddlePowerBase
{
    public GameObject orbitalShieldPrefab; // Prefab del OrbitalShield
    public int initialShieldCount = 3; // Cantidad inicial de OrbitalShields

    private Transform core; // Referencia al núcleo
    private float orbitRadius; // Radio de la órbita del Paddle
    private static List<GameObject> shields = new List<GameObject>(); // Lista global de OrbitalShields activos

    public override void Activate(GameObject paddle)
    {
        Debug.Log("Orbital Shield activated!");

        // Limpia las referencias nulas antes de agregar nuevos escudos
        CleanShieldList();

        // Elimina todos los escudos existentes
        foreach (var shield in shields)
        {
            if (shield != null)
            {
                Destroy(shield);
            }
        }
        shields.Clear();

        // Busca automáticamente el núcleo por su tag
        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        if (coreObject != null)
        {
            core = coreObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró un objeto con el tag 'Core' en la escena.");
            return;
        }

        // Calcula el radio de la órbita basándose en la posición del Paddle
        orbitRadius = Vector3.Distance(core.position, paddle.transform.position);

        // Genera los OrbitalShields iniciales
        for (int i = 0; i < initialShieldCount; i++)
        {
            AddOrbitalShield();
        }
    }

    public void AddOrbitalShield()
    {
        if (core == null || orbitalShieldPrefab == null)
        {
            Debug.LogError("Core o Prefab no asignado.");
            return;
        }

        // Instancia un nuevo OrbitalShield
        GameObject newShield = Instantiate(orbitalShieldPrefab, core.position, Quaternion.identity);
        OrbitalShieldController shieldController = newShield.AddComponent<OrbitalShieldController>();
        shieldController.SetController(this); // Asigna el controlador
        shieldController.core = core;

        // Agrega el OrbitalShield a la lista
        shields.Add(newShield);

        // Reorganiza la posición de todos los OrbitalShields
        UpdateShieldPositions();
    }

    public void RemoveOrbitalShield(GameObject shield)
    {
        if (shields.Contains(shield))
        {
            shields.Remove(shield);

            // Reorganiza la posición de todos los OrbitalShields
            UpdateShieldPositions();
        }
    }

    private void UpdateShieldPositions()
    {
        // Limpia las referencias nulas antes de actualizar posiciones
        CleanShieldList();

        if (shields.Count == 0) return;

        // Distribuye los OrbitalShields equidistantemente en la órbita
        float angleStep = 360f / shields.Count;

        for (int i = 0; i < shields.Count; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPosition = new Vector3(
                core.position.x + Mathf.Cos(angle) * orbitRadius,
                core.position.y + Mathf.Sin(angle) * orbitRadius,
                core.position.z
            );

            // Mueve los escudos suavemente a su nueva posición
            LeanTween.move(shields[i], newPosition, 0.5f).setEaseOutCubic();
        }
    }

    private void CleanShieldList()
    {
        shields.RemoveAll(shield => shield == null);
    }
}
