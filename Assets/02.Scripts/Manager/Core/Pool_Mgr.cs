using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pool_Mgr
{
    class Pool
    {
        public GameObject Original { get; set; }    // Pool을 진행할 대표 변수
        public Transform Root { get; set; }

        Stack<Poolable> m_PoolStack = new Stack<Poolable>();

        // Pool 초기화
        public void Init(GameObject a_Origin, int a_Count = 5)
        {
            Original = a_Origin;
            Root = new GameObject().transform;      // Pool을 담을 Root Object    
            Root.name = $"{a_Origin.name}_Root";

            for (int i = 0; i < a_Count; i++)         // a_Count 만큼 Pool Object 생성 후 Stack에 push
                Push(Create());
        }


        Poolable Create()
        {
            GameObject a_Obj = Object.Instantiate<GameObject>(Original);   
            a_Obj.name = Original.name;
            return a_Obj.GetOrAddComponent<Poolable>();    // Poolable 컴포넌트 생성
        }

        public void PushCreate(int count = 5)
        {
            for (int i = 0; i < count; i++)
                Push(Create());
        }

        // 객체 생성 메소드
        public void Push(Poolable a_Poolable)
        {
            if (a_Poolable.IsNull() == true) return;

            a_Poolable.transform.SetParent(Root);
            a_Poolable.gameObject.SetActive(false);
            a_Poolable.IsUsing = false;

            m_PoolStack.Push(a_Poolable);
        }

        // 객체 반환 메소드
        public Poolable Pop(Transform parent = null)
        {
            Poolable a_Poolable;

            if (m_PoolStack.Count > 0)
                a_Poolable = m_PoolStack.Pop();
            else
                a_Poolable = Create();

            a_Poolable.gameObject.SetActive(true);

            // DontDestroyOnLoad 해제 용도 
            if (parent.IsNull() == true)
                a_Poolable.transform.SetParent(Managers.Scene.m_CurScene.transform);

            a_Poolable.transform.SetParent(parent);
            a_Poolable.IsUsing = true;

            return a_Poolable;
        }
    }

    Dictionary<string, Pool> a_Pool = new Dictionary<string, Pool>();    // Pool 객체 저장
    Transform a_Root;    // 오브젝트 생성 경로

    public void Init()
    {
        // Pool Object를 담을 부모 객체(_root) 경로 설정
        if (a_Root.IsNull() == true)
        {
            a_Root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(a_Root);
        }
    }

    // 새로운 pool 생성 후 저장
    public void CreatePool(GameObject original, int count = 5)
    {
        // 이미 풀에 존재하면 생성 취소
        if (a_Pool.ContainsKey(original.name) == true) return;

        AddCreatePool(original, count);
    }

    public void AddCreatePool(GameObject original, int count = 5)
    {
        if (a_Pool.ContainsKey(original.name) == true)
        {
            a_Pool[original.name].PushCreate(count);
            return;
        }

        Pool pool = new Pool();
        pool.Init(original, count);     // Pool 생성
        pool.Root.SetParent(a_Root);       // _root(@Pool_Root)를 부모 객체로 설정

        a_Pool.Add(original.name, pool);
    }

    // 기존 pool 저장 메소드
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;

        if (a_Pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        a_Pool[name].Push(poolable);
    }

    // pool 반환 메소드
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        // Poolable 컴포넌트가 붙은 객체인데 저장된 Key가 없을 경우 생성
        if (a_Pool.ContainsKey(original.name) == false)
            CreatePool(original);

        return a_Pool[original.name].Pop(parent);        // Pop 진행
    }

    // pool 객체 읽기 메소드
    public GameObject GetOriginal(string a_Name)
    {
        if (a_Pool.ContainsKey(a_Name) == false)
            return null;

        return a_Pool[a_Name].Original;
    }

    // Pool 객체 모두 제거
    public void Clear()
    {
        // @Pool_Root 안에 있는 객체 모두 제거
        foreach (Transform child in a_Root)
            GameObject.Destroy(child.gameObject);

        a_Pool.Clear();  // Pool 초기화
    }
}
