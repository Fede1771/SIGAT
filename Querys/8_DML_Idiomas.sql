USE SIGAT;
GO

INSERT INTO Idioma (Nombre) VALUES ('Español');
INSERT INTO Idioma (Nombre) VALUES ('Portugués');
INSERT INTO Idioma (Nombre) VALUES ('Inglés');
GO

INSERT INTO Control (Control, Form) VALUES ('menu_sistema', 'FrmPrincipal');
INSERT INTO Control (Control, Form) VALUES ('menu_usuarios', 'FrmPrincipal');
INSERT INTO Control (Control, Form) VALUES ('menu_bitacora', 'FrmPrincipal');
INSERT INTO Control (Control, Form) VALUES ('menu_logout', 'FrmPrincipal');
INSERT INTO Control (Control, Form) VALUES ('menu_idioma', 'FrmPrincipal');
INSERT INTO Control (Control, Form) VALUES ('frmbitacora_titulo', 'FrmBitacora');
INSERT INTO Control (Control, Form) VALUES ('lbl_usuario', 'FrmBitacora');
INSERT INTO Control (Control, Form) VALUES ('lbl_actividad', 'FrmBitacora');
INSERT INTO Control (Control, Form) VALUES ('btn_buscar', 'FrmBitacora');
INSERT INTO Control (Control, Form) VALUES ('col_fecha', 'FrmBitacora');
INSERT INTO Control (Control, Form) VALUES ('col_usuario', 'FrmBitacora');
INSERT INTO Control (Control, Form) VALUES ('col_actividad', 'FrmBitacora');
GO

-- Español (Id_Idioma = 1)
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 1, 'Sistema');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 2, 'Gestión de Usuarios');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 3, 'Bitácora');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 4, 'Cerrar Sesión');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 5, 'Idioma');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 6, 'Consulta de Bitácora');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 7, 'Usuario:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 8, 'Actividad:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 9, 'Buscar');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 10, 'Fecha');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 11, 'Usuario');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (1, 12, 'Actividad');

-- Portugués (Id_Idioma = 2)
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 1, 'Sistema');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 2, 'Gestão de Usuários');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 3, 'Registro');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 4, 'Encerrar Sessão');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 5, 'Idioma');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 6, 'Consulta de Registro');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 7, 'Usuário:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 8, 'Atividade:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 9, 'Buscar');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 10, 'Data');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 11, 'Usuário');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (2, 12, 'Atividade');

-- Inglés (Id_Idioma = 3)
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 1, 'System');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 2, 'User Management');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 3, 'Log');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 4, 'Log Out');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 5, 'Language');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 6, 'Log Search');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 7, 'User:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 8, 'Activity:');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 9, 'Search');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 10, 'Date');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 11, 'User');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto) VALUES (3, 12, 'Activity');
GO