namespace NoskGodMod;

partial class NoskFsm : CSFsm<NoskFsm>
{
    public bool isHidden = false;
    public bool p3_wave_dir0_right = false;
    public bool p3_first_hide = true;
    public float p3_wave_size = GetWithLevel(0.16f, 0.32f, 0.32f);
    [FsmState]
    private IEnumerator P3S_Hide()
    {
        DefineEvent("HIDE", nameof(P3Idle));
        DefineEvent("CANCEL", nameof(P3AttackChoice));
        yield return StartActionContent;
        /*var bo = col.bounds;
        if(bo.max.x < PlatformLeftX || bo.min.x > PlatformRightX)
        {
            yield return "CANCEL";
        }
        farawayPlatform = true;
        nextStateAfterLand = nameof(P3S_Hide_0);*/
        if(transform.position.y > 30) yield return "CANCEL";
        yield return anim.PlayAnimWait("Antic");
        ac.PlayOneShot(NoskGod.mimic_spider_jump);
        anim.Play("Jump");
        rig.velocity = new(0, 55);
        col.isTrigger = true;
        rig.isKinematic = true;
        while(transform.position.y < 30) yield return null;
        FSMUtility.SendEventToGameObject(CameraParent.Value, "AverageShake");
        ac.PlayOneShot(NoskGod.mimic_spider_land);
        while(rend.isVisible) yield return null;
        rig.velocity = Vector2.zero;
        rig.isKinematic = true;
        col.isTrigger = false;
        rig.gravityScale = 0;
        transform.position = new(9999999,9999999, 0.4f);
        isHidden = true;
        rend.enabled = false;
        yield return "HIDE";
    }
    [FsmState]
    private IEnumerator P3_Hide_Jump()
    {
        DefineEvent("FALL", "Fall Antic");
        yield return StartActionContent;
        yield return null;
        var hpos = HeroController.instance.transform.position;
        rend.enabled = false;
        hpos.y = 21.3f;
        transform.position = hpos;
        FSMUtility.SendEventToGameObject(CameraParent.Value, "AverageShake");
        anim.Play("Jump");
        rig.isKinematic = false;
        //rig.velocity = new(0, -5);
        //rig.gravityScale = 2;
        rig.position = hpos;
        col.isTrigger = false;
        //ac.PlayOneShot(NoskGod.mimic_spider_jump);
        transform.position = hpos;
        isHidden = false;
        yield return "FALL";
    }
    [FsmState]
    private IEnumerator P3_Hide_Wave_Continuous()
    {
        DefineEvent("IDLE", nameof(P3Idle));
        yield return StartActionContent;
        PlayOneShot(NoskGod.mimic_spider_scream);
        yield return new WaitForSeconds(0.75f);
        wave0.speed = p3_wave_dir0_right ? GetWithLevel(28, 32, 32) : -GetWithLevel(28, 32, 32);
        wave0.loop = false;
        wave0.loopThrough = false;

        wave0.Insert(0, 0, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);

        float timer = 8;
        float stimer = p3_wave_size;
        int st = 0;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            stimer -= Time.deltaTime;
            if(stimer <= 0)
            {
                if(st == 0)
                {
                    st = 1;
                    stimer = p3_wave_size;
                    wave0.Insert(GetWithLevel(10, 11, 12), 0, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);
                }
                else if(st == 1)
                {
                    st = 2;
                    stimer = p3_wave_size * 2;
                    wave0.Insert(0, 0, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);
                }
                else
                {
                    st = 0;
                    stimer = p3_wave_size;
                    wave0.Insert(0.3f, 0, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);
                }
            }
            yield return null;
        }
        yield return wave0.WaitFinish();
        yield return "IDLE";
    }
    [FsmState]
    private IEnumerator P3_Hide_Wave_Hight()
    {
        DefineEvent("IDLE", nameof(P3Idle));
        yield return StartActionContent;
        PlayOneShot(NoskGod.mimic_spider_scream, 1.5f);
        FSMUtility.SendEventToGameObject(CameraParent.Value, "AverageShake");
        wave0.speed = p3_wave_dir0_right ? GetWithLevel(35, 37, 37) : -GetWithLevel(35, 37, 37);
        wave0.loop = false;
        wave0.Insert(0, 0, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);

        yield return new WaitForSeconds(p3_wave_size / 1.5f);

        wave0.Insert(GetWithLevel(22, 28, 28), 0.5f, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);

        yield return new WaitForSeconds(p3_wave_size / 1.5f);

        wave0.Insert(0, 0, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);

        yield return wave0.WaitFinish();

        yield return "IDLE";
    }
    [FsmState]
    private IEnumerator P3_Hide_Wave0()
    {
        DefineEvent("IDLE", nameof(P3Idle));
        yield return StartActionContent;
        PlayOneShot(NoskGod.mimic_spider_scream);
        FSMUtility.SendEventToGameObject(CameraParent.Value, "AverageShake");
        yield return new WaitForSeconds(0.75f);

        wave0.speed = p3_wave_dir0_right ? GetWithLevel(48, 50, 50) : -GetWithLevel(48, 50, 50);
        wave0.loop = true;
        wave0.Insert(0, 0, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);

        yield return new WaitForSeconds(p3_wave_size);

        wave0.Insert(GetWithLevel(15, 16, 17), 0.5f, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);

        yield return new WaitForSeconds(p3_wave_size);

        wave0.Insert(0, 0, p3_wave_dir0_right ? PointDirection.Left : PointDirection.Right);

        yield return new WaitForSeconds(p3_wave_size);
        float timer = GetWithLevel(5, 7, 9);
        while(wave0.points.Count > 2)
        {
            var pp = wave0.points[2];
            var pc = wave0.GetPointPositionInWorld(1).x;
            var hp = HeroController.instance.transform.position.x;
            if(pc < hp)
            {
                wave0.speed += Time.deltaTime * GetWithLevel(48, 58, 58);
            }
            else
            {
                wave0.speed -= Time.deltaTime * GetWithLevel(48, 58, 58);
            }
            pp.position.y -= Time.deltaTime;
            timer -= Time.deltaTime;
            if(timer <= 0) break;
            wave0.points[2] = pp;
            
            wave0.speed = Mathf.Clamp(wave0.speed, -48, 48);
            yield return null;
        }
        wave0.speed = wave0.speed > 0 ? 48 : -48;
        wave0.loop = false;
        yield return wave0.WaitFinish();
        wave0.speed = 0;
        yield return "IDLE";
    }
    [FsmState]
    private IEnumerator P3HideAttackChoice()
    {
        DefineEvent("JUMP OUT", nameof(P3_Hide_Jump));
        DefineEvent("WAVE 0", nameof(P3_Hide_Wave0));
        DefineEvent("WAVE FIRST", nameof(P3_Hide_Wave_Continuous));
        DefineEvent("WAVE HIGH", nameof(P3_Hide_Wave_Hight));
        SendRandomEventV3 eventer = new();
        eventer.events = new[]
        {
            FsmEvent.GetFsmEvent("JUMP OUT"),
            FsmEvent.GetFsmEvent("WAVE 0"),
            FsmEvent.GetFsmEvent("WAVE HIGH")
        };
        eventer.weights = new FsmFloat[]
        {
            0.25f,
            0.25f,
            0.25f
        };
        eventer.trackingInts = new FsmInt[]
        {
            0,
            0,
            0
        };
        eventer.eventMax = new FsmInt[]{
            1,
            1,
            1
        };
        eventer.trackingIntsMissed = new FsmInt[]
        {
            0,
            0,
            0
        };
        eventer.missedMax = new FsmInt[]
        {
            3,
            8,
            4
        };
        yield return StartActionContent;
        if(p3_first_hide)
        {
            p3_first_hide = false;
            yield return "WAVE FIRST";
        }
        yield return eventer;
    }
}