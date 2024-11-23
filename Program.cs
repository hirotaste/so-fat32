using System;
using System.Collections.Generic;

class Program
{
    // Quantidade total de clusters disponíveis no "disco" (sistema de arquivos)
    const int TotalClusters = 128;
    // Tamanho de cada cluster em bytes (como se cada bloco tivesse 512 bytes)
    const int ClusterSize = 512;

    // Tabela FAT que indica o próximo cluster de cada arquivo
    // -1 significa que o cluster está livre, qualquer outro valor indica o próximo cluster de um arquivo
    static int[] fatTable = new int[TotalClusters];
    
    // "Disco" onde os dados dos arquivos serão armazenados. Cada posição do array representa um cluster.
    static byte[][] disk = new byte[TotalClusters][];

    // Função para inicializar o sistema de arquivos (preparar a tabela FAT e o disco)
    static void InitializeFileSystem()
    {
        // Configura cada cluster como vazio (-1) e cria espaço para armazenar dados em cada cluster
        for (int i = 0; i < TotalClusters; i++)
        {
            fatTable[i] = -1;  // -1 indica que o cluster está livre
            disk[i] = new byte[ClusterSize]; // Cria um espaço de 512 bytes para cada cluster
        }
    }

    // Função para criar um novo arquivo e armazenar seus dados no disco
    static int CreateFile(string data)
    {
        // Converte o conteúdo do arquivo em bytes (cada letra vira um número em bytes)
        byte[] fileData = System.Text.Encoding.UTF8.GetBytes(data);
        
        // Calcula quantos clusters são necessários para armazenar o arquivo
        int clustersNeeded = (fileData.Length + ClusterSize - 1) / ClusterSize;

        // Lista para armazenar clusters livres encontrados
        List<int> freeClusters = new List<int>();
        
        // Procura clusters livres na tabela FAT
        for (int i = 0; i < TotalClusters && freeClusters.Count < clustersNeeded; i++)
        {
            if (fatTable[i] == -1) // -1 significa cluster livre
            {
                freeClusters.Add(i); // Adiciona o cluster à lista de clusters livres
            }
        }

        // Se não houver clusters suficientes, o arquivo não pode ser criado
        if (freeClusters.Count < clustersNeeded)
        {
            Console.WriteLine("Erro: Espaço insuficiente para o arquivo.");
            return -1;
        }

        // Preenche a tabela FAT e armazena dados no disco
        for (int i = 0; i < clustersNeeded; i++)
        {
            int currentCluster = freeClusters[i]; // Cluster atual para armazenar parte do arquivo
            int nextCluster = (i < clustersNeeded - 1) ? freeClusters[i + 1] : -2; // -2 indica o fim do arquivo

            fatTable[currentCluster] = nextCluster; // Marca o próximo cluster na tabela FAT

            // Define qual parte do arquivo será copiada para o cluster atual
            /*
                O offset é calculado multiplicando o índice do cluster (i) pelo tamanho de cada cluster (ClusterSize). Isso indica o ponto de início dos dados que devem ser copiados para o cluster atual.
                Por exemplo, se ClusterSize é 512 bytes:
                Quando i = 0, offset = 0 * 512 = 0 (ou seja, copiamos a partir do início do arquivo).
                Quando i = 1, offset = 1 * 512 = 512 (copiamos a partir do byte 512, ou o segundo "bloco" de dados).
                E assim por diante, cada cluster começa onde o anterior terminou, garantindo que os dados sejam divididos corretamente.
            */
            int offset = i * ClusterSize;

            int bytesToCopy = Math.Min(ClusterSize, fileData.Length - offset);
            
            // Copia a parte dos dados para o cluster atual
            Array.Copy(fileData, offset, disk[currentCluster], 0, bytesToCopy);
        }

        Console.WriteLine($"Arquivo criado ocupando {clustersNeeded} clusters.");
        return freeClusters[0]; // Retorna o primeiro cluster do arquivo criado
    }

    // Função para ler o conteúdo de um arquivo usando o primeiro cluster
    static void ReadFile(int startCluster)
    {
        // Verifica se o cluster inicial é válido
        if (startCluster < 0 || startCluster >= TotalClusters || fatTable[startCluster] == -1)
        {
            Console.WriteLine("Arquivo não encontrado.");
            return;
        }

        List<byte> fileData = new List<byte>(); // Lista para armazenar dados lidos do arquivo
        int currentCluster = startCluster;

        // Continua lendo até encontrar o final do arquivo (-2)
        while (currentCluster != -2)
        {
            fileData.AddRange(disk[currentCluster]); // Adiciona os dados do cluster atual à lista de dados
            currentCluster = fatTable[currentCluster]; // Pega o próximo cluster a partir da tabela FAT
        }

        // Converte os dados de bytes para texto e remove espaços vazios
        string data = System.Text.Encoding.UTF8.GetString(fileData.ToArray()).TrimEnd('\0');
        Console.WriteLine("Conteúdo do arquivo: " + data); // Mostra o conteúdo do arquivo
    }

    // Função principal para executar a simulação
    static void Main()
    
    {
        InitializeFileSystem(); // Inicializa o sistema de arquivos

        Console.WriteLine("Simulação do sistema de arquivos FAT32.");
        Console.Write("Digite o conteúdo do arquivo a ser criado: ");
        string data = Console.ReadLine(); // Lê o texto digitado pelo usuário

        // Cria o arquivo e obtém o primeiro cluster ocupado
        int startCluster = CreateFile(data);

        // Se o arquivo foi criado com sucesso, lê e mostra o conteúdo
        if (startCluster != -1)
        {
            Console.WriteLine($"Arquivo criado no cluster {startCluster}. Lendo arquivo...");
            ReadFile(startCluster); // Lê o conteúdo do arquivo
        }

        //evolução do sistema
        /*
            Com base na aula do nosso primeiro encontro tente implementar as seguintes funcionalidades:
            - Crie um Menu no qual o usuário poderá escolher se quer Inserir, listar listar arquvos inseridos,
            visualizar o conteúdo do arquivo ou sair do sistema
            - Crie as execuções para as funcionalidades definidas
            Observação: Utilize 2 vetores para armazenar o nome do arquivo inserido e a primeira possição
            do arquivo no cluster
        */
        
    }
}
