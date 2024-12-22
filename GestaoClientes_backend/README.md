As variáveis de ambiente estão protegidas nos arquivos appsettings de cada projeto (Username, Password e URL). Atualmente, seus valores são "admin", "password" e "https://localhost:7037/".

Caso haja alteração nas chaves de login em um dos projetos, é necessário aplicar a mesma alteração no outro.

Configurações de banco de dados esta variavel DefaultConnection presente no arquivo appsettings do backend(Altere conforme o seu acesso ao banco).

Para garantir a segurança, foi implementado o método de autenticação JWT nos endpoints de inserção, exclusão e edição.

O front-end foi desenvolvido utilizando ASP.NET MVC, com Razor e JavaScript, enquanto o back-end foi construído como uma API REST em C# com .NET Core 8.0.

Quanto ao banco de dados, utilizei SQL Server e Entity Framework no método GET de cada projeto, além de Procedures nos demais métodos.
A arquitetura segue os padrões MVC e microserviços.

O projeto tem como objetivo realizar operações de CRUD para Clientes e Logradouros, estabelecendo uma relação de um para muitos entre as entidades.

Segue exemplos de comandos que usei para criar as procedures de Clientes e Logradouras:

----------------------CLIENTES------------------------------------
CREATE PROCEDURE ClientePROCEDURE
    @Id UNIQUEIDENTIFIER,
    @Nome NVARCHAR(255),
    @Email NVARCHAR(255),
    @Logotipo NVARCHAR(MAX),
    @Logradouros NVARCHAR(MAX), -- JSON de logradouros
    @Operacao NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar operação
    IF @Operacao = 'Inserir'
    BEGIN
        INSERT INTO Clientes (Id, Nome, Email, Logotipo)
        VALUES (@Id, @Nome, @Email, @Logotipo);

        -- Inserir logradouros (se fornecidos)
        IF @Logradouros IS NOT NULL
        BEGIN
            INSERT INTO Logradouros (IdLogradouro, IdCliente, Rua, Quadra, Lote, Numero, Bairro)
            SELECT 
                CAST(JSON_VALUE(value, '$.IdLogradouro') AS UNIQUEIDENTIFIER),  -- Garantir que IdLogradouro é UNIQUEIDENTIFIER
                CAST(JSON_VALUE(value, '$.IdCliente') AS UNIQUEIDENTIFIER),     -- Usar IdCliente do JSON
                JSON_VALUE(value, '$.Rua'),
                JSON_VALUE(value, '$.Quadra'),
                JSON_VALUE(value, '$.Lote'),
                JSON_VALUE(value, '$.Numero'),
                JSON_VALUE(value, '$.Bairro')
            FROM OPENJSON(@Logradouros);
        END
    END
    ELSE IF @Operacao = 'Editar'
    BEGIN
        -- Atualizar dados do cliente
        UPDATE Clientes
        SET 
            Nome = @Nome,
            Email = @Email,
            Logotipo = @Logotipo
        WHERE Id = @Id;

        -- Atualizar logradouros (se fornecidos)
        IF @Logradouros IS NOT NULL
        BEGIN
            -- Excluir logradouros antigos
            DELETE FROM Logradouros WHERE IdCliente = @Id;

            -- Inserir novos logradouros
            INSERT INTO Logradouros (IdLogradouro, IdCliente, Rua, Quadra, Lote, Numero, Bairro)
            SELECT 
                CAST(JSON_VALUE(value, '$.IdLogradouro') AS UNIQUEIDENTIFIER),
                CAST(JSON_VALUE(value, '$.IdCliente') AS UNIQUEIDENTIFIER), -- Usar IdCliente do JSON
                JSON_VALUE(value, '$.Rua'),
                JSON_VALUE(value, '$.Quadra'),
                JSON_VALUE(value, '$.Lote'),
                JSON_VALUE(value, '$.Numero'),
                JSON_VALUE(value, '$.Bairro')
            FROM OPENJSON(@Logradouros);
        END
    END
    ELSE IF @Operacao = 'Remover'
    BEGIN
        -- Excluir logradouros associados
        DELETE FROM Logradouros WHERE IdCliente = @Id;

        -- Excluir cliente
        DELETE FROM Clientes WHERE Id = @Id;
    END
END;
GO

----------------------LOGRADOURAS------------------------------------

CREATE PROCEDURE Logradouro_PROCEDURE
    @IdLogradouro UNIQUEIDENTIFIER = NULL,      -- Identificador do logradouro (null para inserção)
    @IdCliente UNIQUEIDENTIFIER,                -- Identificador do cliente (necessário)
    @Rua NVARCHAR(255),                         -- Nome da rua
    @Quadra NVARCHAR(50),                       -- Nome da quadra
    @Lote NVARCHAR(50),                         -- Nome do lote
    @Numero INT = NULL,                         -- Número da residência (Agora pode ser NULL)
    @Bairro NVARCHAR(255),                      -- Nome do bairro
    @Operacao NVARCHAR(50)                      -- Operação (Inserir, Atualizar, Remover)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operacao = 'Inserir'
    BEGIN
        -- Inserir um novo logradouro
        INSERT INTO Logradouros (IdLogradouro, IdCliente, Rua, Quadra, Lote, Numero, Bairro)
        VALUES (NEWID(), @IdCliente, @Rua, @Quadra, @Lote, @Numero, @Bairro);
        
        -- Retornar o logradouro inserido
        SELECT * FROM Logradouros WHERE IdCliente = @IdCliente ORDER BY IdLogradouro DESC;
    END
    ELSE IF @Operacao = 'Atualizar'
    BEGIN
        -- Atualizar o logradouro existente
        UPDATE Logradouros
        SET Rua = @Rua,
            Quadra = @Quadra,
            Lote = @Lote,
            Numero = @Numero,
            Bairro = @Bairro
        WHERE IdLogradouro = @IdLogradouro;

        -- Retornar o logradouro atualizado
        SELECT * FROM Logradouros WHERE IdLogradouro = @IdLogradouro;
    END
    ELSE  IF @Operacao = 'Remover'
    BEGIN
        -- Retorna o logradouro antes de removê-lo
        SELECT * FROM Logradouros WHERE IdLogradouro = @IdLogradouro;

        -- Remove o logradouro
        DELETE FROM Logradouros WHERE IdLogradouro = @IdLogradouro;
    END
END;
GO
