using System.Collections;
using UnityEngine;

public class AlphaTestManager : MonoBehaviour
{
    [SerializeField] private GameObject Dummy;
    [SerializeField] private EnemyHealth script;

    private void Awake()
    {
        script = Dummy.gameObject.GetComponent<EnemyHealth>();    
    }

    void Update()
    {
        if (script.health <= 0)
        {
            StartCoroutine(SpawnNewEnemy());  
        }
    }

    IEnumerator SpawnNewEnemy()
    {
        yield return new WaitForSeconds(1f);
        bool spawnEnemy = true;

        if (spawnEnemy == true)
        {
            spawnEnemy = false;
            Instantiate(Dummy, new Vector3(0, 0.9f, 6), new Quaternion(0, 1, 0, 0));
        }
        yield break;
    }
}
