using System;
using System.IO;
using System.Threading;

class Program
{
    // Paylaşılan kaynaklar
    static readonly object resourceA = new object();
    static readonly object resourceB = new object();

    // Log yazımı için kilit
    static readonly object logLock = new object();

    // Random için kilit
    static readonly object randomLock = new object();
    static readonly Random random = new Random();

    // Deadlock durumu için kilit
    static readonly object stateLock = new object();
    static bool deadlockDetected = false;

    // Log dosyasını proje klasörüne yaz
    static string logFilePath = Path.GetFullPath(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\log.txt")
    );

    static void Main(string[] args)
    {
        File.WriteAllText(logFilePath, "===== Deadlock Simülatörü Log Dosyası =====\n\n");

        Console.WriteLine("===== Deadlock Simülatörü =====");
        Console.WriteLine("1 - Deadlock Oluşturan Senaryo");
        Console.WriteLine("2 - Deadlock Önlenmiş Senaryo");
        Console.WriteLine("3 - Timeout ile Deadlock Tespiti");
        Console.WriteLine("4 - İstatistiksel Test");
        Console.Write("Seçiminizi girin: ");

        string choice = Console.ReadLine();
        Console.WriteLine();

        Log("Log dosyası konumu: " + logFilePath);

        switch (choice)
        {
            case "1":
                RunDeadlockScenario();
                break;

            case "2":
                RunSafeScenario();
                break;

            case "3":
                RunTimeoutDetectionScenario();
                break;

            case "4":
                RunStatisticsScenario();
                break;

            default:
                Log("Geçersiz seçim yapıldı.");
                break;
        }

        Log("Program sonlandı.");
        Console.WriteLine("\nÇıkmak için bir tuşa basın...");
        Console.ReadKey();
    }

    static void Log(string message)
    {
        string fullMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";

        lock (logLock)
        {
            Console.WriteLine(fullMessage);
            File.AppendAllText(logFilePath, fullMessage + Environment.NewLine);
        }
    }

    static void RandomSleep(string threadName)
    {
        int time;

        lock (randomLock)
        {
            time = random.Next(200, 1000);
        }

        Log($"{threadName}: {time} ms bekliyor...");
        Thread.Sleep(time);
    }

    static void MarkDeadlock(string message)
    {
        lock (stateLock)
        {
            deadlockDetected = true;
        }

        Log(message);
    }

    // 1) Deadlock oluşturan klasik sürüm
    static void RunDeadlockScenario()
    {
        Log("=== Deadlock Oluşturan Senaryo Başladı ===");

        Thread thread1 = new Thread(Thread1DeadlockWork);
        Thread thread2 = new Thread(Thread2DeadlockWork);

        thread1.Name = "Thread-1";
        thread2.Name = "Thread-2";

        thread1.Start();
        thread2.Start();

        // NOT: Bu senaryoda deadlock oluşursa program burada bekler.
        thread1.Join();
        thread2.Join();

        Log("Deadlock senaryosu sona erdi.");
    }

    // 2) Deadlock önlenmiş sürüm
    static void RunSafeScenario()
    {
        Log("=== Deadlock Önlenmiş Senaryo Başladı ===");

        Thread thread1 = new Thread(SafeThreadWork);
        Thread thread2 = new Thread(SafeThreadWork);

        thread1.Name = "Thread-1";
        thread2.Name = "Thread-2";

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();

        Log("Deadlock önlenmiş senaryo başarıyla tamamlandı.");
    }

    // 3) Timeout ile deadlock tespiti
    static void RunTimeoutDetectionScenario()
    {
        lock (stateLock)
        {
            deadlockDetected = false;
        }

        Log("=== Timeout ile Deadlock Tespiti Başladı ===");

        Thread thread1 = new Thread(Thread1TimeoutWork);
        Thread thread2 = new Thread(Thread2TimeoutWork);

        thread1.Name = "Thread-1";
        thread2.Name = "Thread-2";

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();

        bool detected;
        lock (stateLock)
        {
            detected = deadlockDetected;
        }

        if (detected)
        {
            Log("Sonuç: Deadlock tespit edildi.");
        }
        else
        {
            Log("Sonuç: Deadlock tespit edilmedi.");
        }

        Log("Timeout tabanlı deadlock tespit senaryosu tamamlandı.");
    }

    // 4) İstatistiksel test
    static void RunStatisticsScenario()
    {
        Log("=== İstatistiksel Test Başladı ===");

        int totalRuns = 10;
        int deadlockCount = 0;
        int successCount = 0;
        long totalDuration = 0;

        for (int i = 1; i <= totalRuns; i++)
        {
            lock (stateLock)
            {
                deadlockDetected = false;
            }

            Log($"\n--- Deneme {i} başladı ---");

            DateTime startTime = DateTime.Now;

            Thread thread1 = new Thread(Thread1TimeoutWork);
            Thread thread2 = new Thread(Thread2TimeoutWork);

            thread1.Name = $"Thread-1 (Deneme {i})";
            thread2.Name = $"Thread-2 (Deneme {i})";

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            DateTime endTime = DateTime.Now;
            long duration = (long)(endTime - startTime).TotalMilliseconds;
            totalDuration += duration;

            bool detected;
            lock (stateLock)
            {
                detected = deadlockDetected;
            }

            if (detected)
            {
                deadlockCount++;
                Log($"Deneme {i} sonucu: Deadlock tespit edildi.");
            }
            else
            {
                successCount++;
                Log($"Deneme {i} sonucu: Başarıyla tamamlandı.");
            }

            Log($"Deneme {i} süresi: {duration} ms");
        }

        double deadlockRate = (double)deadlockCount / totalRuns * 100.0;
        double averageDuration = (double)totalDuration / totalRuns;

        Log("\n=== İstatistik Sonuçları ===");
        Log($"Toplam deneme sayısı: {totalRuns}");
        Log($"Deadlock tespit edilen deneme sayısı: {deadlockCount}");
        Log($"Başarıyla tamamlanan deneme sayısı: {successCount}");
        Log($"Deadlock oluşma oranı: %{deadlockRate:F2}");
        Log($"Ortalama deneme süresi: {averageDuration:F2} ms");
    }

    // 1. senaryo için deadlock oluşturan threadler
    static void Thread1DeadlockWork()
    {
        Log($"{Thread.CurrentThread.Name}: Resource-A alınmak isteniyor.");

        lock (resourceA)
        {
            Log($"{Thread.CurrentThread.Name}: Resource-A alındı.");
            RandomSleep(Thread.CurrentThread.Name);

            Log($"{Thread.CurrentThread.Name}: Resource-B alınmak isteniyor.");

            lock (resourceB)
            {
                Log($"{Thread.CurrentThread.Name}: Resource-B alındı.");
                Log($"{Thread.CurrentThread.Name}: İşlem tamamlandı.");
            }
        }
    }

    static void Thread2DeadlockWork()
    {
        Log($"{Thread.CurrentThread.Name}: Resource-B alınmak isteniyor.");

        lock (resourceB)
        {
            Log($"{Thread.CurrentThread.Name}: Resource-B alındı.");
            RandomSleep(Thread.CurrentThread.Name);

            Log($"{Thread.CurrentThread.Name}: Resource-A alınmak isteniyor.");

            lock (resourceA)
            {
                Log($"{Thread.CurrentThread.Name}: Resource-A alındı.");
                Log($"{Thread.CurrentThread.Name}: İşlem tamamlandı.");
            }
        }
    }

    // 2. senaryo için güvenli thread
    static void SafeThreadWork()
    {
        Log($"{Thread.CurrentThread.Name}: Resource-A alınmak isteniyor.");

        lock (resourceA)
        {
            Log($"{Thread.CurrentThread.Name}: Resource-A alındı.");
            RandomSleep(Thread.CurrentThread.Name);

            Log($"{Thread.CurrentThread.Name}: Resource-B alınmak isteniyor.");

            lock (resourceB)
            {
                Log($"{Thread.CurrentThread.Name}: Resource-B alındı.");
                Log($"{Thread.CurrentThread.Name}: İşlem tamamlandı.");
                RandomSleep(Thread.CurrentThread.Name);
            }

            Log($"{Thread.CurrentThread.Name}: Resource-B bırakıldı.");
        }

        Log($"{Thread.CurrentThread.Name}: Resource-A bırakıldı.");
    }

    // 3 ve 4. senaryo için timeout tabanlı threadler
    static void Thread1TimeoutWork()
    {
        bool lockA = false;
        bool lockB = false;

        try
        {
            Log($"{Thread.CurrentThread.Name}: Resource-A alınmak isteniyor.");

            lockA = Monitor.TryEnter(resourceA, 2000);

            if (!lockA)
            {
                Log($"{Thread.CurrentThread.Name}: Resource-A alınamadı. Timeout oluştu.");
                return;
            }

            Log($"{Thread.CurrentThread.Name}: Resource-A alındı.");
            RandomSleep(Thread.CurrentThread.Name);

            Log($"{Thread.CurrentThread.Name}: Resource-B alınmak isteniyor.");

            lockB = Monitor.TryEnter(resourceB, 2000);

            if (!lockB)
            {
                Log($"{Thread.CurrentThread.Name}: Resource-B için timeout oluştu.");
                MarkDeadlock("Deadlock detected: Thread-1 ikinci kaynağı alamadı.");
                return;
            }

            Log($"{Thread.CurrentThread.Name}: Resource-B alındı.");
            Log($"{Thread.CurrentThread.Name}: İşlem tamamlandı.");
        }
        finally
        {
            if (lockB)
            {
                Monitor.Exit(resourceB);
                Log($"{Thread.CurrentThread.Name}: Resource-B bırakıldı.");
            }

            if (lockA)
            {
                Monitor.Exit(resourceA);
                Log($"{Thread.CurrentThread.Name}: Resource-A bırakıldı.");
            }
        }
    }

    static void Thread2TimeoutWork()
    {
        bool lockB = false;
        bool lockA = false;

        try
        {
            Log($"{Thread.CurrentThread.Name}: Resource-B alınmak isteniyor.");

            lockB = Monitor.TryEnter(resourceB, 2000);

            if (!lockB)
            {
                Log($"{Thread.CurrentThread.Name}: Resource-B alınamadı. Timeout oluştu.");
                return;
            }

            Log($"{Thread.CurrentThread.Name}: Resource-B alındı.");
            RandomSleep(Thread.CurrentThread.Name);

            Log($"{Thread.CurrentThread.Name}: Resource-A alınmak isteniyor.");

            lockA = Monitor.TryEnter(resourceA, 2000);

            if (!lockA)
            {
                Log($"{Thread.CurrentThread.Name}: Resource-A için timeout oluştu.");
                MarkDeadlock("Deadlock detected: Thread-2 ikinci kaynağı alamadı.");
                return;
            }

            Log($"{Thread.CurrentThread.Name}: Resource-A alındı.");
            Log($"{Thread.CurrentThread.Name}: İşlem tamamlandı.");
        }
        finally
        {
            if (lockA)
            {
                Monitor.Exit(resourceA);
                Log($"{Thread.CurrentThread.Name}: Resource-A bırakıldı.");
            }

            if (lockB)
            {
                Monitor.Exit(resourceB);
                Log($"{Thread.CurrentThread.Name}: Resource-B bırakıldı.");
            }
        }
    }
}