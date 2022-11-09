using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beyblade : MonoBehaviour
{
    PlayerControls playerControls;
    [SerializeField] private Material MyMat;
    [SerializeField] private Material MyMatAbort;
    [SerializeField] private Material KeyMat;
    [SerializeField] private ParticleSystem Particlesys;
    [SerializeField] [ColorUsage(true, true)] private Color cSt = Color.red;
    [SerializeField] [ColorUsage(true, true)] private Color cEn = Color.blue;
    [SerializeField] [ColorUsage(true, true)] private Color cMd = Color.white;
    [SerializeField] [ColorUsage(true, true)] private Color cSt2 = Color.red;
    [SerializeField] private Color LColorSt = Color.red;
    [SerializeField] private Color LColorEn = Color.blue;
    [SerializeField] private Light Abort;
    [SerializeField] private Light BayBld;
    [SerializeField] private Transform PlayerAbort;
    [SerializeField] private Transform BeyBlado;
    [SerializeField] private Transform Abort2;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rigidbody AboRB;
    [SerializeField] private GameObject Cube;
    [SerializeField] private NewBehaviourScript1 SWT;
    [SerializeField] private SphereCollider SPC;
    [SerializeField] private BoxCollider BXC;
    [SerializeField] private BayAct BA;
    [SerializeField] private velocitaangolare Va;
    [SerializeField] private GameObject PExplosion;
    private float Min = -2f;
    private float Max = 2f;
    private float radius = 10.0F;
    private float power = 120.0F;
    private bool isExploding = false;
    private bool Jump;
    private bool CorouFin = true;
    private bool isSit = false;
    public bool canSit = false;
    private float speed = 15;
    private float lerp;
    private float t = 0.0f;
    private Vector3 Vjump = new Vector3(0.0f, 2.0f, 0.0f);
    private Vector2 move;
    private float jumpForce = 300f;
    private bool isGrounded = true;
    
    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.BasicMovement.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        playerControls.BasicMovement.Move.canceled += ctx => NormalValuo2();
        playerControls.BasicMovement.Jump.started += ctx => Jump = true;
        playerControls.BasicMovement.Jump.canceled += ctx => Jump = false;
        playerControls.BasicMovement.Enter.performed += ctx => Corou();
        playerControls.BasicMovement.Special.performed += ctx => Explode1();
    }

    void FixedUpdate()
    {
        BeyBlado.eulerAngles = new Vector3(Mathf.Clamp(transform.rotation.eulerAngles.x, Min, Max),
        transform.rotation.eulerAngles.y,
        Mathf.Clamp(transform.rotation.eulerAngles.z, Min, Max));

        if(move.y > 0.2f && isSit)
        {
            rb.AddForce(Cube.transform.forward * speed * move.y);
            NormalValuo();
        }
        if(move.y < -0.2f && isSit)
        {
            rb.AddForce(-Cube.transform.forward * speed * -move.y);
            NormalValuo();
        }
        if(move.x < -0.2f && isSit)
        {
            rb.AddForce(-Cube.transform.right * speed * -move.x);
            NormalValuo();
        }
        if(move.x > 0.2f && isSit)
        {
            rb.AddForce(Cube.transform.right * speed * move.x);
            NormalValuo();
        }
        if(Jump && isGrounded && isSit)
        {   
            GetComponent<Rigidbody>().AddForce (Vjump * jumpForce *Time.deltaTime, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void Update()
    {
        if(isSit)
        {
            PlayerAbort.transform.position = Abort2.transform.position;
        }
    }

    private void Corou()
    {
        if(canSit && CorouFin)
        {
            CorouFin = false;
            isSit = true;
            StartCoroutine(Updtbal());
        }
        if(!canSit && CorouFin)
        {
            CorouFin = false;
            isSit = false;
            StartCoroutine(UnUpdtbal());
        }
    }
    private void Explode1()
    {
        if(!isExploding && isSit)
        {
            StartCoroutine(Explode());
            isExploding = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            Particlesys.Play();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
            Particlesys.Stop();
        }
    }

    private void NormalValuo()
    {
        Min = -4f;
        Max = 4f;
    }
    private void NormalValuo2()
    {
        move = Vector2.zero;
        Min = -2f;
        Max = 2f;
    }

    private void MyFunction()
    {
        t += 1 * Time.deltaTime;
        lerp = Mathf.Lerp(0, 1, t);
        MyMat.color = Color.Lerp(cSt, cEn, lerp);
        MyMat.SetColor("_EmissionColor", Color.Lerp(cSt, cEn, lerp));
        MyMatAbort.SetColor("_EmissionColor", Color.Lerp(cEn, cSt2, lerp));
        KeyMat.SetColor("_EmissionColor", Color.Lerp(cMd, cSt, lerp));
        BayBld.color = Color.Lerp(LColorSt, LColorEn, lerp);
        Abort.color = Color.Lerp(LColorEn, LColorSt, lerp);
        while(t > 1)
        {
            t = 1;
        }
    }
    private void MyFunction2()
    {
        t += -0.5f * Time.deltaTime;
        lerp = Mathf.Lerp(0, 1, t);
        MyMat.color = Color.Lerp(cSt, cEn, lerp);
        MyMat.SetColor("_EmissionColor", Color.Lerp(cSt, cEn, lerp));
        MyMatAbort.SetColor("_EmissionColor", Color.Lerp(cEn, cSt2, lerp));
        KeyMat.SetColor("_EmissionColor", Color.Lerp(cMd, cSt, lerp));
        BayBld.color = Color.Lerp(LColorSt, LColorEn, lerp);
        Abort.color = Color.Lerp(LColorEn, LColorSt, lerp);
        while(t < 0)
        {
            t = 0;
        }
    }

    private IEnumerator Explode()
    {
        PExplosion.SetActive(true);
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
        }
        yield return new WaitForSeconds(5);
        PExplosion.SetActive(false);
        isExploding = false;
    }

    private IEnumerator Updtbal()
    {
        BA.enabled = false;
        SPC.enabled = false;
        BXC.enabled = false;
        AboRB.useGravity = false;
        Va.enabled = true;
        SWT.enabled = false;
        this.gameObject.tag="Player";
        InvokeRepeating("MyFunction", 0, 0.01f);
        yield return new WaitForSeconds(2);
        CancelInvoke("MyFunction");
        canSit = false;
        CorouFin = true;
    }
    
    private IEnumerator UnUpdtbal()
    {
        BA.enabled = true;
        SPC.enabled = true;
        BXC.enabled = true;
        AboRB.useGravity = true;
        Va.enabled = false;
        SWT.enabled = true;
        this.gameObject.tag="Untagged";
        InvokeRepeating("MyFunction2", 0, 0.01f);
        yield return new WaitForSeconds(2);
        CancelInvoke("MyFunction2");
        CorouFin = true;
    }
    
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
}
