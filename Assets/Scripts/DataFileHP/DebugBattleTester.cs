using UnityEngine;
using System.Reflection; // ใช้สำหรับเรียก Action ภายนอก

public class DebugBattleTester : MonoBehaviour
{
    void Update()
    {
        // กด M = จำลองการกดโดนปกติ (Hit)
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("[Test] Simulating: OnNoteHit");
            // เราต้องใช้ Reflection หรือสร้าง Method ใน NoteObject เพื่อเรียก Invoke 
            // แต่เพื่อความง่าย ผมจะสร้าง Class เล็กๆ มาส่งสัญญาณแทน
            TestTrigger.InvokeHit();
        }

        // กด N = จำลองการกดแบบ Perfect
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("[Test] Simulating: OnNotePerfectHit");
            TestTrigger.InvokePerfect();
        }

        // กด B = จำลองการกดพลาด (Miss)
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("[Test] Simulating: OnNoteMiss");
            TestTrigger.InvokeMiss();
        }
    }
}

// ตัวช่วยส่งสัญญาณ (ใส่ไว้ท้ายไฟล์ DebugBattleTester.cs)
public static class TestTrigger {
    public static void InvokeHit() {
        // ดึง Event มาจาก NoteObject ที่คุณเขียนไว้
        var type = typeof(NoteObject);
        var eventField = type.GetField("OnNoteHit", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (eventField != null) {
            var action = (System.Action)eventField.GetValue(null);
            action?.Invoke();
        }
    }
    public static void InvokePerfect() {
        var type = typeof(NoteObject);
        var eventField = type.GetField("OnNotePerfectHit", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (eventField != null) {
            var action = (System.Action)eventField.GetValue(null);
            action?.Invoke();
        }
    }
    public static void InvokeMiss() {
        var type = typeof(NoteObject);
        var eventField = type.GetField("OnNoteMiss", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (eventField != null) {
            var action = (System.Action)eventField.GetValue(null);
            action?.Invoke();
        }
    }
}