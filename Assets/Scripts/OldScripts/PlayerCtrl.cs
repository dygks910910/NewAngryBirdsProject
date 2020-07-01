using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerCtrl : MonoBehaviour {
    public GameObject[] birds;
    public GameObject StartPos;
    public GameObject slider;
    private Slider Slidervalue;
    [SerializeField]private Vector2 Direction;
    [SerializeField]private float Power = 0;
    private int CountBird = 0;
    [SerializeField]private bool DirectionSetting = false;
    [SerializeField]private bool PowerSetting = false;

    private AudioSource source;
    public AudioClip fireSound;

    public GameObject DirectionObject;

	// Use this for initialization
	void Start () {
        for(int i = 1; i < birds.Length; ++i)
        {
            birds[i].SetActive(false);
        }
        Slidervalue = slider.GetComponent<Slider>();
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        //if(Input.GetMouseButtonDown(0))
        //{
        //    Vector3 mousePos = Input.mousePosition;
        //    mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        //    Direction = mousePos - birds[0].transform.position;
        //    Direction = Direction.normalized;
        //    //birds[0].GetComponent<Rigidbody2D>().AddForce(fireDirection.normalized * 400);
        //}
        if(Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            Direction = mousePos - birds[CountBird].transform.position;
            Direction = Direction.normalized;

            float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
            birds[CountBird].transform.Rotate(0,0,angle);
            //print(angle);

            DirectionSetting = true;
        }
        if(DirectionSetting)
        {
            Slidervalue.value += 10;
            if(Slidervalue.value >= 500)
            {
                Slidervalue.value= 0;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space) && DirectionSetting)
        {
            Power = Slidervalue.value;
            PowerSetting = true;
        }
        if(PowerSetting && DirectionSetting)
        {
            FireBird();
        }
        PowerSetting = false;
    }


    void FireBird()
    {
        source.clip = fireSound;
        source.loop = false;
        source.Play();
        birds[CountBird].GetComponent<Rigidbody2D>().AddForce(Direction * Power);
       
            Destroy(birds[CountBird], 10.0f);
        if(CountBird < birds.Length-1)
        {
            CountBird++;
            birds[CountBird].SetActive(true);
        }
            
        PowerSetting = false;
        DirectionSetting = false;
        Slidervalue.value = 0;
        Power = 0;
        Direction = new Vector2(0, 0);
    }
   
}
