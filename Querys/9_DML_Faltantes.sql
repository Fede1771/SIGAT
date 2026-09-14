USE SIGAT;
GO

DECLARE @idEspanol INT = (SELECT Id FROM Idioma WHERE Nombre = 'Español');
DECLARE @idPortugues INT = (SELECT Id FROM Idioma WHERE Nombre = 'Portugués');
DECLARE @idIngles INT = (SELECT Id FROM Idioma WHERE Nombre = 'Inglés');

-- ===== Lo que faltaba en FrmBitacora =====
DECLARE @idChkFechas INT, @idColId INT, @idColInfo INT;

INSERT INTO Control (Control, Form) VALUES ('chk_fechas', 'FrmBitacora');
SET @idChkFechas = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('col_id', 'FrmBitacora');
SET @idColId = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('col_informacion', 'FrmBitacora');
SET @idColInfo = SCOPE_IDENTITY();

INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol, @idChkFechas, 'Fechas:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues, @idChkFechas, 'Datas:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles, @idChkFechas, 'Dates:');

INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol, @idColId, 'Id');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues, @idColId, 'Id');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles, @idColId, 'Id');

INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol, @idColInfo, 'Información');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues, @idColInfo, 'Informação');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles, @idColInfo, 'Information');
GO

-- ===== Pantalla FrmGestionUsuarios (nueva) =====
DECLARE @idEspanol2 INT = (SELECT Id FROM Idioma WHERE Nombre = 'Español');
DECLARE @idPortugues2 INT = (SELECT Id FROM Idioma WHERE Nombre = 'Portugués');
DECLARE @idIngles2 INT = (SELECT Id FROM Idioma WHERE Nombre = 'Inglés');

DECLARE @idTitulo INT, @idLblUsuario INT, @idLblClave INT, @idLblNombre INT, @idLblApellido INT,
        @idLblPerfil INT, @idChkActivo INT, @idBtnGuardar INT, @idBtnEliminar INT, @idBtnLimpiar INT,
        @idColIdUsuario INT, @idColNombreUsuario INT, @idColNombre INT, @idColApellido INT,
        @idColActivo INT, @idColPerfil INT;

INSERT INTO Control (Control, Form) VALUES ('frmgestionusuarios_titulo', 'FrmGestionUsuarios');
SET @idTitulo = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('lbl_usuario_gu', 'FrmGestionUsuarios');
SET @idLblUsuario = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('lbl_clave', 'FrmGestionUsuarios');
SET @idLblClave = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('lbl_nombre', 'FrmGestionUsuarios');
SET @idLblNombre = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('lbl_apellido', 'FrmGestionUsuarios');
SET @idLblApellido = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('lbl_perfil', 'FrmGestionUsuarios');
SET @idLblPerfil = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('chk_activo', 'FrmGestionUsuarios');
SET @idChkActivo = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('btn_guardar', 'FrmGestionUsuarios');
SET @idBtnGuardar = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('btn_eliminar', 'FrmGestionUsuarios');
SET @idBtnEliminar = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('btn_limpiar', 'FrmGestionUsuarios');
SET @idBtnLimpiar = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('col_idusuario', 'FrmGestionUsuarios');
SET @idColIdUsuario = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('col_nombreusuario', 'FrmGestionUsuarios');
SET @idColNombreUsuario = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('col_nombre', 'FrmGestionUsuarios');
SET @idColNombre = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('col_apellido', 'FrmGestionUsuarios');
SET @idColApellido = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('col_activo', 'FrmGestionUsuarios');
SET @idColActivo = SCOPE_IDENTITY();
INSERT INTO Control (Control, Form) VALUES ('col_perfil', 'FrmGestionUsuarios');
SET @idColPerfil = SCOPE_IDENTITY();

-- Español
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idTitulo, 'Gestión de Usuarios');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idLblUsuario, 'Usuario:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idLblClave, 'Clave (vacío no cambia):');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idLblNombre, 'Nombre:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idLblApellido, 'Apellido:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idLblPerfil, 'Perfil:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idChkActivo, 'Activo');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idBtnGuardar, 'Guardar');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idBtnEliminar, 'Baja');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idBtnLimpiar, 'Limpiar');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idColIdUsuario, 'Id');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idColNombreUsuario, 'Usuario');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idColNombre, 'Nombre');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idColApellido, 'Apellido');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idColActivo, 'Activo');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idEspanol2, @idColPerfil, 'Perfil');

-- Portugués
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idTitulo, 'Gestão de Usuários');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idLblUsuario, 'Usuário:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idLblClave, 'Senha (vazio não altera):');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idLblNombre, 'Nome:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idLblApellido, 'Sobrenome:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idLblPerfil, 'Perfil:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idChkActivo, 'Ativo');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idBtnGuardar, 'Salvar');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idBtnEliminar, 'Baixa');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idBtnLimpiar, 'Limpar');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idColIdUsuario, 'Id');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idColNombreUsuario, 'Usuário');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idColNombre, 'Nome');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idColApellido, 'Sobrenome');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idColActivo, 'Ativo');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idPortugues2, @idColPerfil, 'Perfil');

-- Inglés
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idTitulo, 'User Management');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idLblUsuario, 'User:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idLblClave, 'Password (blank = no change):');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idLblNombre, 'Name:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idLblApellido, 'Last name:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idLblPerfil, 'Role:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idChkActivo, 'Active');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idBtnGuardar, 'Save');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idBtnEliminar, 'Deactivate');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idBtnLimpiar, 'Clear');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idColIdUsuario, 'Id');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idColNombreUsuario, 'Username');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idColNombre, 'First name');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idColApellido, 'Last name');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idColActivo, 'Active');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (@idIngles2, @idColPerfil, 'Role');
GO