# Improve Apply Memory Hacking - FreeFire
Funcional em todas versões, inclusive na atual - 1.93.X
- Faça um bom aproveito!
- Já que eu não faço código como dizem por ai
- Código desenvolvido por Lucas...
- Dizem que não crio nada, mas se aproveitaram de um code que mandei e agora estão usando em sua gambiarra.
- Impossível a garena fazer uma correção.

## Todos os créditos é para Lucas!
### Autor: Lucass#0004
Discord: [Discord Oficial - Lucass#0004]

## Como esse improve apply funciona?

bom, ele vai suspender o processo do bluestacks enquanto está aplicando um Hex. Dificultando a garena de detectar um memory hacking e diminuindo o tempo de aplicação.
+ Simples, né?
+ Para melhorar mais ainda você pode usar um code para aumentar a prioridade do processo do cheat,

![image-TBQktRu](https://i.imgur.com/TBQktRu.png)
![image-uiyw5Jj](https://i.imgur.com/uiyw5Jj.png)

+ como vocês podem ver, no começo do metodo rep ele suspende o processo do bluestacks.

![image-5KOKVaq](https://i.imgur.com/5KOKVaq.png)


+ Talvez vocês também precisem adicionar...

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);
        
### Divirta-se!!!!
