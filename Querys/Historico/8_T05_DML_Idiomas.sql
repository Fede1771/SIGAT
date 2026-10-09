/* =====================================================================
   T05 - GESTIÓN DE MÚLTIPLES IDIOMAS (SIGAT)
   Script DML - Datos de arranque REALES del proyecto
   (menú de FrmPrincipal, pantalla FrmBitacora y FrmGestionIdiomas)
   DVH = SHA-256(Id_Idioma|Id_Control|Texto), mismo criterio que
   HashHelper.ObtenerHashSHA256 usado en el resto de SIGAT.
   ===================================================================== */

USE SIGAT;
GO

SET IDENTITY_INSERT dbo.Idioma ON;
GO
INSERT INTO Idioma (Id, Nombre) VALUES (1, N'Español');
INSERT INTO Idioma (Id, Nombre) VALUES (2, N'Portugués');
INSERT INTO Idioma (Id, Nombre) VALUES (3, N'Ruso');
INSERT INTO Idioma (Id, Nombre) VALUES (4, N'Chino Simplificado');
INSERT INTO Idioma (Id, Nombre) VALUES (5, N'Alemán');
INSERT INTO Idioma (Id, Nombre) VALUES (6, N'Francés');
INSERT INTO Idioma (Id, Nombre) VALUES (7, N'Árabe');
GO
SET IDENTITY_INSERT dbo.Idioma OFF;
GO

SET IDENTITY_INSERT dbo.Control ON;
GO
INSERT INTO Control (Id, Control, Form) VALUES (1, N'menu_sistema', N'FrmPrincipal');
INSERT INTO Control (Id, Control, Form) VALUES (2, N'menu_usuarios', N'FrmPrincipal');
INSERT INTO Control (Id, Control, Form) VALUES (3, N'menu_bitacora', N'FrmPrincipal');
INSERT INTO Control (Id, Control, Form) VALUES (4, N'menu_logout', N'FrmPrincipal');
INSERT INTO Control (Id, Control, Form) VALUES (5, N'menu_idioma', N'FrmPrincipal');
INSERT INTO Control (Id, Control, Form) VALUES (6, N'menu_gestion_idiomas', N'FrmPrincipal');
INSERT INTO Control (Id, Control, Form) VALUES (7, N'titulo_usuario', N'FrmPrincipal');
INSERT INTO Control (Id, Control, Form) VALUES (8, N'titulo_perfil', N'FrmPrincipal');
INSERT INTO Control (Id, Control, Form) VALUES (9, N'frmbitacora_titulo', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (10, N'chk_fechas', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (11, N'lbl_usuario', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (12, N'lbl_actividad', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (13, N'btn_buscar', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (14, N'col_id', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (15, N'col_fecha', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (16, N'col_usuario', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (17, N'col_actividad', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (18, N'col_informacion', N'FrmBitacora');
INSERT INTO Control (Id, Control, Form) VALUES (19, N'frmgestionidiomas_titulo', N'FrmGestionIdiomas');
INSERT INTO Control (Id, Control, Form) VALUES (20, N'lbl_idioma_activo', N'FrmGestionIdiomas');
INSERT INTO Control (Id, Control, Form) VALUES (21, N'btn_aplicar_idioma', N'FrmGestionIdiomas');
INSERT INTO Control (Id, Control, Form) VALUES (22, N'lbl_nuevo_idioma', N'FrmGestionIdiomas');
INSERT INTO Control (Id, Control, Form) VALUES (23, N'btn_nuevo_idioma', N'FrmGestionIdiomas');
INSERT INTO Control (Id, Control, Form) VALUES (24, N'lbl_edicion', N'FrmGestionIdiomas');
INSERT INTO Control (Id, Control, Form) VALUES (25, N'lbl_nombre_form', N'FrmGestionIdiomas');
INSERT INTO Control (Id, Control, Form) VALUES (26, N'lbl_nombre_control', N'FrmGestionIdiomas');
INSERT INTO Control (Id, Control, Form) VALUES (27, N'lbl_texto', N'FrmGestionIdiomas');
INSERT INTO Control (Id, Control, Form) VALUES (28, N'btn_guardar_traduccion', N'FrmGestionIdiomas');
GO
SET IDENTITY_INSERT dbo.Control OFF;
GO

-- Idioma: Español
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 1, N'Sistema', 'c9c8d34af3a4bb72fdc5362a11c6f9c94ba770ba626dbdc37285e16bbd15bfce');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 2, N'Gestión de Usuarios', 'b7633d92c8e057ddfe04e48eff897ba9d85a0a23c6e40fc9162ba54e242ea8b8');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 3, N'Bitácora', 'c541e0e847898bb7ec9b0bc96ddea01aea50569de2468a09a06acfb652dd5ad8');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 4, N'Cerrar Sesión', '81aa25049e4a6ef5f6221b49139f758fd030bc0338e84e31d44e19782b535a86');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 5, N'Idioma', 'fcf363d47f55c42769207bfba1727733c87b19e67df720debaa994b62b5ceaed');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 6, N'Gestionar idiomas...', 'b28b8bc345bfa5cd5c3cf983ec82106a5e68108b1a5137fee9535955d7b443c6');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 7, N'Usuario', 'a860929fd4b8efd838a4632bdd86c0c2f41ff64d89303aa7f2326b9a888ade5a');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 8, N'Perfil', '07698898996e73927c80626789ebebc2f53776ddd71df2fc52228d7ff2dad22e');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 9, N'Consulta de Bitácora', 'c445626e3078b27ae62573a2a382a22246e56a5d7c1fbb1bf5300dc1e09fafbb');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 10, N'Fechas:', '8017e46a2560c3eea2f0c8495190c37b26d27d019474dbf32fdfbc1cd95537d2');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 11, N'Usuario:', '44f28918cb0a6690ee4230903164196d6c820a597b30b0e597d14960d32db9b1');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 12, N'Actividad:', '5eef000a5e92188cba0b4bd87e5861a3a7970e7bffefe0a0cd3c83425f59f0fe');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 13, N'Buscar', 'f7d338161d1ed2098d7e38ad172e7fff4b1ee0fb12c5b5c604bd65e61fe8d649');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 14, N'Id', '34bcd2a2bd9c2a3e6bc3b65dc6fe79fd829b3278a9c6f66e1d034c0cb11433ae');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 15, N'Fecha', '7b59d6103939f0ac3bf34a353029fb1a50fb42076c82d6b2e7c75e4f53345a50');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 16, N'Usuario', '3e247334ae330c1355c5dddbc6fef15569c20c5fad3da4e9236e3d4e4f556f10');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 17, N'Actividad', 'a6d20329df89fd5df1d5392689d693ab0e08dfc4c560ce4ee8550730e660a2eb');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 18, N'Información', '705e834a10770b8e437d5e9d45b76d21597ab4c9a4b60cb6488758bf88827381');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 19, N'Gestión de idiomas', '1fcc670d19696e875c47781a797a16fd814145eae9127e82999e9fdc6fa2faae');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 20, N'Idioma activo:', '86929cf7af989c5a5ac45633f72fcaa54499feebc05f71e082a27eaff9346a83');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 21, N'Aplicar a todo SIGAT', '6df89e86007c598824a566b9b0c281731b0840a3f3b409878e335194798b06b5');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 22, N'Nuevo idioma:', '6741a77bd71c14f6aa5f4d587fe26dc082da89fc6216fe780af4cad2d07c57b3');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 23, N'Crear (clona Español)', '8e650624347259655413592c1b50f8a35a4bce7bc0a1a3f4de8a9d72c6d0fee6');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 24, N'Editar / crear una traducción', 'ec3b8b745621d64fe8c17a481c8fc4c6a57566e897ff3fd2a59e7f05cc6bea5f');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 25, N'Formulario (Form):', 'd1bfdde3757fc4f9eec3ef4fbd3a22980b73d76b849f5177164f3dfd44a4dcfc');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 26, N'Control (Tag lógico):', '7a23b9f8ff8d5be01d210803c28b3bf335bed6684353aaf78df8bb9ad47e44d1');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 27, N'Texto traducido:', '119dbcfd7f3b185f01c2701226a85e2077e7e397e7a1ada9e40d58246ef6bf72');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (1, 28, N'Guardar traducción', '853d6e63956ef12832c421db8d571be51145568dd9b406ee9f93ef09fb7816ca');

-- Idioma: Portugués
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 1, N'Sistema', '535df85bb841bce052465b3364d834e187ed442f8729a37d5b091a548f834324');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 2, N'Gestão de Usuários', 'af678d0099abe8d008fcdb539b5328699d5a14c4732cc96e17c83d6f992c64e6');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 3, N'Registro', '7306168c49e9e5a90f89f0f6e6068604d7bfaf315a0a8aa75ecc05ce54bf15fe');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 4, N'Encerrar Sessão', '7ba54c931111dd3d93ab7fa094e2c771fcc1e189ece00adf22d8319d10be0b0e');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 5, N'Idioma', '56374036870fc25db4515d9c9381315798dba22a39034b5c145b97973f38725e');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 6, N'Gerenciar idiomas...', '1d7af91f961995200cc1f4c6454dd922abe3419295f0f2c0c7232c4f49ce4728');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 7, N'Usuário', '8d984477808bc994a9a5b2a536e4ad7c33e21d47a7bd3338508ebf02147c7a7f');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 8, N'Perfil', '3d80d445bfc819df748a1c8f5b7d774c33e2d075246af64713e19a35a69f8e29');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 9, N'Consulta de Registro', 'f0980d34cfa24f0521906a2cdeff343fc5316b11d511f06daa16eb6d989bd2bb');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 10, N'Datas:', '5fe6487834f3a64e0a96e28c03cf80d3c1bbc2688b427fcdf269400329eac952');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 11, N'Usuário:', '399de81a34b2746402a7c1643056807df9da396914c4a7b42537e354da108055');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 12, N'Atividade:', 'cddc1a198769c769de539fd3bd1bf745cd8565399c843454ab996a8fd9a020fe');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 13, N'Buscar', '338e0f3e07a9e6c7e1b0ee8f57656e2e51d100b88bd94de6de06bfa9e4087a0e');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 14, N'Id', '43a35f754cbc1ce38363bfb790f783951ea519641ecc6008367b6e52c6616209');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 15, N'Data', '0e6f212972b740a08e603a39d88521a11cc527c9bf47b0a8309ef59bb97dacc0');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 16, N'Usuário', '6b0df65312b25f957ef0a1a5d070526df2f09bb796ab0e749611a59372126739');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 17, N'Atividade', 'adc66b6534130f946462ba74e9a20964c0acdce1416e6ce9fac5ebf611c3fbf4');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 18, N'Informação', 'f5b2ffd9b881519bea71d2d63ae8b091a83f38e78961b684f006596f43e85454');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 19, N'Gestão de idiomas', 'a5f9a9012a5462954cd4dc05ad89d7f90df8837aa347457e9b06e82581ad41cb');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 20, N'Idioma ativo:', 'a06f5b749351bf09259a787ccde3b13b55f2d4de4ea76df83fccb47295c8379c');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 21, N'Aplicar a todo o SIGAT', 'b77e668a81c3ad12b1172dca41886d2b3656e924bf947687a9a62cd3d62143ed');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 22, N'Novo idioma:', '984ff2a3337afe8d23f2a14d966a6a30342f68aa3412736416f175989a0ff84e');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 23, N'Criar (clona Espanhol)', '741ec9569c9f327df1a45dfd7c1a2a0841a5acc23c822fa6a9e2e7c525dc8823');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 24, N'Editar / criar uma tradução', '9471275a59edba0f9d8cbdc283aba120574007c041a0a85bb66f402cceed716a');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 25, N'Formulário (Form):', '1771bfd6100361b26cb42e08342c83aa0c1a15d9c5d58282c8a9bb8b47ccbc38');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 26, N'Controle (Tag lógica):', '640bd68c60a77bf06e06eba9ff06606a4875d310948e47ab6898b66287e9b7bc');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 27, N'Texto traduzido:', '39839015a6d5f3619ebd96e89f3311946893796488a9467bf560ec3a492a0f66');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (2, 28, N'Salvar tradução', '226c718545d7a324a89afdd32fcbc8b924de4961f5498d431cf88d55e3de831f');

-- Idioma: Ruso
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 1, N'Система', '6724d3e553a805a73b3165e6bebcaf5afe1a8051b5057dfa6b7d55975578fb9b');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 2, N'Управление пользователями', '843c2b46df46a52a50c4cbf31e51b9e3fd7823a7b7b97551f2cea96659fdb630');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 3, N'Журнал', 'c5554e101cf9dc6cc95ff072ced01aa008f50e02bddd0cb4233de42ab37d4ee0');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 4, N'Выйти', '8e4279a5b8fb2ebf5aee4b8daba557861076335014b972848b7251ae5b8243a3');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 5, N'Язык', '77d32de1a0c324f27524babdcda31930c4d8ef1d9195de2658d89efa27cfca2b');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 6, N'Управление языками...', 'b28c997fb86ccd1006a1933d02775296b4ada340ba30b1514140287bacd82f58');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 7, N'Пользователь', 'a94bc0aeb4d0bd49c5639ae66e77ce81d05e312bea1cbf26907e5b936bbacd7b');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 8, N'Профиль', 'a7ee4611e8783a551629c4d68d59291c395a376b3f5976fa11542f7a8cf9f06c');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 9, N'Просмотр журнала', '5262cbccebd29226f360a55483438ceadfdd795ad72930f650a7bcaa8b908f52');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 10, N'Даты:', '1762e87b412d5bb6f462adb264faff81fbbd08b0c5b35a7f5cd7b9b91cc7dd6a');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 11, N'Пользователь:', 'e461db85292bbf83c8197bfb07e9125718ad2d944ef8b0e47bb5d2a848ad2861');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 12, N'Действие:', '424ad1a4617f944f9724e56a9df6da0979d8f85f8ace3b358742c04338255c6c');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 13, N'Поиск', 'f3d9ce79f3a7389d8a1b6292f824914aea76b84ba7abf5e0640101cf5a1d92e6');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 14, N'Ид', '7853e417e76a1871c63e36a2f0b2d52b6443b5be2bd4270f0fd111ab83be2a66');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 15, N'Дата', 'bdb738ddce2d45bb8edd99b11f0a846c9a4e694d0ab03ab0b259094cc3d0406f');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 16, N'Пользователь', '9f9cd290a677eb2cfe93878f05b5287e5091029afc563500ae524a81fab2258a');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 17, N'Действие', '92b2cb0afeaf2cf9e99bcc3f1b5d1f7e8d05b6491c3f67229719b9eaa98d942a');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 18, N'Информация', '28aa7d0c1b2b326d37a7f77767edbf0cd09eebf5995093777384829cb612be80');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 19, N'Управление языками', '39e16a8998d531732ae8a8743bc9e21808dd114fa911bd5757dd4580d359b19f');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 20, N'Активный язык:', '3a2ff7462a1fe7c36d0fa33ba389ee8442147c1f281854264b006dc967f69637');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 21, N'Применить ко всей системе SIGAT', '93514532172fbd872bfb7c3882c184917b82b42b903087be42d4091407521b3b');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 22, N'Новый язык:', 'dc57b8cec50cd87e16728dc899fc556a20413318bd3ca3c0197b4ecf267160c7');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 23, N'Создать (копия испанского)', '169560e6d91b9c8a47cf0cbb2d7b419d128ac04f22e876ddd1d7384034737daf');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 24, N'Изменить / создать перевод', '8de6327b3d42b7947377afb899f009a6bc6e37cfbd1a7fb09863d0035ba72c65');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 25, N'Форма (Form):', 'ba887abfb7e63d836179e7cf9c11f4da96407610ba2a50facef95b1b5bf1a981');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 26, N'Элемент (логический Tag):', '83a144f81c63a0397abe1336325bbea8bd99bd857c11897281e934523c914d13');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 27, N'Переведённый текст:', 'd93af5a03a04261c5eb52aa0f24eb5d433a1ecd11fac7075ce512695a3315333');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (3, 28, N'Сохранить перевод', '6d026427987775430582c085305da0aab366881bfd9702867498cdd072a03228');

-- Idioma: Chino Simplificado
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 1, N'系统', 'faa850c53e17f613020e222d95a3695acf3ea1441e4a2ee1dfeab63961e39dbc');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 2, N'用户管理', '9865c25319edbfd733991a4e1639cc75c78d83c15305672051d0975c942e2ecb');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 3, N'日志', 'b28ce2134c2d7bc360d56a879426243173d46930c0a033ec89c58bbcd1ca66bb');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 4, N'注销', '36f9e35c176530d6dde92435022e078a85c4d3d2f09b2ec8a4259dc821371546');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 5, N'语言', 'baaf7952a4f6595ebd8c0d4d5c41d8cceb414801a4a88fc50b7ef18321382880');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 6, N'管理语言...', '38e6628e1004695d35212852a51baeb9bbbcc887cbd03c5db40dae14a61b17d0');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 7, N'用户', 'a50f00b70a4afcc519e4f71be4abd4ad6987fa3c94ba877dc63a052ed9554337');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 8, N'角色', '73b913817e1321bfcacc520c3f32bc4f094e5b0550a24b7d247c4f0d104fc492');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 9, N'日志查询', '01800bb121d61ae0d35afe26a1eb18f8a0838414d9c9fc8b8ecc4ec446e1eef7');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 10, N'日期：', '9b92af0113cc6b55931db7ddae8c23add9540ed729b0414e2e2ddca8701c93a3');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 11, N'用户：', '1429feb2e901383f292ef7215aaee550a08a41cdcea432840d23ed8d8713d580');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 12, N'活动：', 'ab4d17aa4330a86e05609fd62d4b4f61d63732cb3bd389c24b0bf183e3b88ae8');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 13, N'搜索', '2b8bfcd95ab88c0116b54ce1102d826e0bc9ffec64d08044561dd8ad6b1f7a48');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 14, N'编号', '4b451cae7750694379d2904e9ee97da2cdebb72c4e01f9cf9b7191fd63006cca');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 15, N'日期', '309431af62d2ee0a8cccb7d51aa0c4f505326046acc6f880ba41eb8857c0c537');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 16, N'用户', '2208e32ea93ff506faae7959f5734dd8d701c7f518e9376a72e16ebe6a5a4893');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 17, N'活动', 'f76f87e257c66b1d893d5ebea825397387531623a2745a0d836e0ae6e1c847a4');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 18, N'信息', '06fbcc8712c1951ce4a34770998e94ee52ac85413bedf8ba8f174f8589ee2b4d');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 19, N'语言管理', 'de781fc59f903907eada6aaadf9989b40f33a6dfa7b815616aa7e67723207e21');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 20, N'当前语言：', '2e7fe956c6a78fb38b7dcb9718cc7438fa75b70063b2eaf1000b55209b257bf5');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 21, N'应用到整个SIGAT系统', '5914cae9e0b8350ef8c35e49413cfd317e5c72ace70c06b73ab176e9851587cf');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 22, N'新语言：', '412d7248b25d6e9a3a5300c3464af712bffe41749b2d7dd8daa80caf6a28df31');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 23, N'创建（复制西班牙语）', '7ccc9a1c051f88929696b5215321a2b34f70aaf53a8b8ff8dd6ed55b1f691f20');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 24, N'编辑/创建翻译', '5f612380d62d4b35779a5f7b38776ddf0ee68d17cfb9711784495542d6f2f449');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 25, N'窗体（Form）：', '10f9b4b4e721b706fd677a2f59bb36a2bc99fe6996055f1eace1a34e8c545026');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 26, N'控件（逻辑Tag）：', '990b75ee1a8361252df70259bb42d30e5b90b9f679e44a465e700bb7c1004bcf');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 27, N'翻译文本：', '89feb6a9ecb8a4c48571e3a4cad9d86e6599054b47550ed85a058f92d56482d9');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (4, 28, N'保存翻译', '5fe9be159abf5b1e6487c54bff45311b15caf5c0774d5843ac77f3daea3a6245');

-- Idioma: Alemán
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 1, N'System', 'f01edb64d0b949eb1476b3dd0434c56af0d4225e66933134a737cf9999544680');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 2, N'Benutzerverwaltung', '79dbde17cbaa3abe9707bd153bf7a143495f68dfd38bf89a7857bfa97d2047c8');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 3, N'Protokoll', 'f95aa8217e287820ad1187b124663c027e94d8fd6b14311a706c9149c190c582');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 4, N'Abmelden', '91671bb5b9f68cbc0c25c1de144494c9cffb08c09d6ea8adfc78dab558781171');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 5, N'Sprache', '67bade7b285fa96cf88a09c11d5469e2d7289fea1f139d8d9841838dc7aa569f');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 6, N'Sprachen verwalten...', 'af1882a98f479bd402daacba9fba7dcc54b06acdd970e61596a84c0b816fa037');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 7, N'Benutzer', '048bb7e0da5e5b155814c281bcc1425bc0e8cddefa9af01d3205b58d029d32ac');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 8, N'Profil', '949082820ef9b57ca15640a9910139f604dfff82f1b8cec05f1cc33fff862a53');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 9, N'Protokollabfrage', '6c3640c74e7ec54391b9bdf427d81ec6dec9ed8a87ee4b82a85b5a1cf3eeaab0');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 10, N'Daten:', '76839b6f9e1e46e9819596873bf64be304ca0c72de60719808a6295a7d19ae46');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 11, N'Benutzer:', '6aa74a9e40619927534b0414bf4ede10e107d2846f199b0d1b1e0a56014a8647');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 12, N'Aktivität:', '18ff590be1dcef1504235521d520abfb096ea0008a4d12abae891cced81fe0d5');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 13, N'Suchen', '31a81e2e1f2e65317efdfc54bda73248b4e2701bcc303b82d696f6cf05fa1c01');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 14, N'Id', 'f0e7a11ce9978fc5f9a98a5a4bda0b2124339159e9068f9579e50981a02f5a69');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 15, N'Datum', '0093b6b2b4b36777739251e5afff26b152aa6925efbeeb4f7bbc89d1de70a808');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 16, N'Benutzer', '991c17430c14eed236b3dc70c6e377e42e2b21a9ab3c93295af7955bd008f960');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 17, N'Aktivität', 'b00f2866e1b8f8666da63d7080605d22fc3c1c9cce428df745c8895f3a2fc0a6');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 18, N'Information', '5956dffdb9ea4291d8f2d40d3d1835da821a380d94c03b966268c6074ec45492');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 19, N'Sprachverwaltung', 'a4489b23c57f338f335a2f0b34d73f06bae2ce33c35bc96d3ded44714b591736');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 20, N'Aktive Sprache:', '902d737d3307b6de5bf5be2adf9485c2e3eb39d82801d6d8df33900abca581a3');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 21, N'Auf ganz SIGAT anwenden', 'd47e3144ebb8e289ddedd9e7e1d52a1bc78a95e7cdeb7966b27f27194188b3db');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 22, N'Neue Sprache:', 'f2d4f1b0f79c4a4d542323378241b450f3edbcabc43afdace4fdb6477d426ff9');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 23, N'Erstellen (kopiert Spanisch)', '110831cdeb719bcaf968b45e16b5b48d1d2e0839ba665fee9e75b6bf22883a17');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 24, N'Übersetzung bearbeiten/erstellen', 'b6e20eb8ce23ad8ff42eaaf8c722fc713122f816832bf41c1828386640f772c4');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 25, N'Formular (Form):', '073376cb2b5944f219180cce8e0b3ec9a2404e3e3574969a5c6c4210eb80fc77');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 26, N'Steuerelement (logisches Tag):', '51eb2cfbf87b24a2c5abcd1a8b67f27cadbe0d41cbbac184bffb1524120c77a5');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 27, N'Übersetzter Text:', '2b9a2e87e264ae7965a963e6e7b9a0db444d519ce3ea947ba11558c81be0843c');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (5, 28, N'Übersetzung speichern', '9dc224cb4c9239ea483e09c0efcdaac85cb3a8d6a8a967acf8cc5688c4f073b6');

-- Idioma: Francés
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 1, N'Système', 'd315e5aa94eeb73b72f944ec35213885db74aa56e38f323b6b2b65c3c4a51054');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 2, N'Gestion des utilisateurs', 'ac049c462f7f8bff6c80380776db71288b17009d027a35fed838b37d9d3c7f83');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 3, N'Journal', '937517ffede519d1aa13ea49b5cf59357c36dcb25f1396ebd1cb5430b07c0d56');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 4, N'Déconnexion', 'daa56f002cccd6448302995c9422ebde88d1587ac7e7459e4c7ea7294a5982d4');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 5, N'Langue', 'cd9eabce554f5c44dd5edef7b45ef17b5862406b57a1040aca8f070ce76f4d87');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 6, N'Gérer les langues...', '049b49688169cfa33bbe98b66e9555edc51917c42fba4b25fed84dbe51130a8b');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 7, N'Utilisateur', '2dd477b324045c29c14bd8bb340b7aa20f558525e1888a9d551aa1cdb0f9e07d');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 8, N'Profil', 'bca9d7ae0ebeb581b5128b4c1e16f7d7c5f17b47170136c269b8572626fe0abd');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 9, N'Consultation du journal', 'fa64d99ae42031c9472be7946591ec52417a35eeef99a504ebc3e1728cce6b2d');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 10, N'Dates :', '82b69ccc297a8c8353757f6bc1d6b619855bab3041e86166a57b8dbae657c859');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 11, N'Utilisateur :', '878cfb1bcb4d14ea5147001f1c3ae30b014c41b0cca9c3bbb85c4051bc32dae0');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 12, N'Activité :', '175c5f6697167fc965cd2aace583ca815f391ed97ae33619a6c3872973c96b89');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 13, N'Rechercher', '89296afca1f3183a559638158686cb0a6c5afb3ae524ab7c7970fe9583c44a57');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 14, N'Id', 'e263ef043fd4e0d08e9477380379bac012c8ed241daebdf13e6b49a81fc04b9f');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 15, N'Date', 'd3a80d7e04b2fba7f4377ad196c27f05fa5a426cb0df4a9675dc8a2a31c9b4f0');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 16, N'Utilisateur', '6ee36570d39118e27c9f6d130abefb090d4beb91a129338327509b7b33417553');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 17, N'Activité', 'b89a45bff37dc390ed8e98e840165436c2804bc8a4c77e76e56d0b3c74a6c41c');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 18, N'Information', '210813eb8e98e29241e3f6e28e9b9dcc57212249a87dcca1a6c973c2a151f35c');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 19, N'Gestion des langues', '2719aaf215fe81a184d4f5a159f69a4d9024746c12e9f8bdafc632dc85bd1fd4');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 20, N'Langue active :', '7abd81d676c48ae4c5fbb91b6c0739a8c356f9bbe00faf874b8eb80a0ef9b6dc');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 21, N'Appliquer à tout SIGAT', 'd24822540e77859925c2d76b423bde645680c2cd984b7bb87b87d82be7729336');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 22, N'Nouvelle langue :', '2ff55192163b00572384fa06529dcf89db9385e20a1a8d4aa6e44fb4b3befa44');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 23, N'Créer (clone l''espagnol)', '760aafb9b4f2957dc52e6a924a59d2e78e8d02c2d5820751362e7d2ab0e8f936');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 24, N'Modifier / créer une traduction', 'b5c289f968effb285acd9ef6b5868353d6835b202b725fc1158a468d39170b01');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 25, N'Formulaire (Form) :', 'ae936fa1bbb43b05ef3501e00aab8a0614f10a63893ee7d52257f8317e70805f');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 26, N'Contrôle (Tag logique) :', '678cb7413738faf660623b60e570e877217ffa4177c66d9589b7f5536c550eb3');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 27, N'Texte traduit :', '6e2d2449bfdedfde83e147310f40ea2f67b656129de1769c40d831b01d569aa3');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (6, 28, N'Enregistrer la traduction', 'e9fcca360eb51b612a999360eacb8e44c0274ec4db2a470e4dd6a60acfa019aa');

-- Idioma: Árabe
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 1, N'النظام', 'e30a00a5a7b3cd18f86c7b3dcf4913d35e698ed7c84324db1a7f11f21f0b254b');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 2, N'إدارة المستخدمين', '78101ce0010412cd50edfbb57e950f76407928c8dab5aabb94751fc8089afa1b');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 3, N'السجل', '98946c6f9fe3a30682eadbe050872b6af6f3ee101bb0f8df51588622d0b49953');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 4, N'تسجيل الخروج', 'd1927c271fa81badab0a619c2e9c7f4646e086482c66d3c1d6a8016c609ae5f9');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 5, N'اللغة', 'adaf67014ededf9f22b0cf2aeda2bbd4a552aa59ea64f18f23a19caa756f03b3');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 6, N'إدارة اللغات...', '6d751c38d86d976b87af825d26dc469c25d06cb483e9311feb22e3ab973cef87');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 7, N'المستخدم', 'e3ba1a7c551e5689c3ca3dc006b5c0ed98081d3069df4f5bf46e699d6df9f489');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 8, N'الملف الشخصي', '9970ec0910cd03cb034caf600968accaf1e2837de45325ef0e2c7f205c831eac');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 9, N'استعلام السجل', 'a92fe65aefd278c970c0f3b7bd429bb3a79dbfa3ce3c0aa1be42f54181c9c311');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 10, N'التواريخ:', 'bcd88f1a75ef9b9d136d2ca5a363ebfea5076f187427a345fbef205de7ef0520');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 11, N'المستخدم:', '1a04272ec3e10153027bbfd3524d9ebd7c40901e890e92d42bcc19e0142ca4f3');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 12, N'النشاط:', '50e9d907aff2c76127e20cc4ac9380b75bc1427ed4d347f748c8f0820d8df612');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 13, N'بحث', '1937ab5bec644bf3e68a0e2b59ac1e754b5824eb45d7709079728df7d8e10c63');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 14, N'المعرف', '38f0b3841b12006f709f90975a63fdd86d2948c90f14ac8964c342a2808ec6c2');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 15, N'التاريخ', '538d2b1358a10cb0434853f7283fee3130461bc8bb2a643d7be14ea7dd475cbc');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 16, N'المستخدم', 'afbf7edcad47cfecca2a95d4aa22e6e0f84fb622ab168290f3e2c1ce69656698');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 17, N'النشاط', 'daf4d2bfd5a172b38965c253962151b447dd3e879efef22e17edac5937946aa8');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 18, N'المعلومات', '3429e06262c5f6137037d000d3b24ca99bfabac63ee55936073feb8b07e6e116');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 19, N'إدارة اللغات', 'f6995b20907d91ebf7f138cca674fc17c064c3e035698e17e3f1d5585dfb077c');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 20, N'اللغة النشطة:', '93e302496412991d81bd29b84c6cc3c5de1be21a5ff62952d8eaa73d0802ca00');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 21, N'تطبيق على نظام SIGAT بالكامل', 'c6eb21a87c2e57cc6ad3a9df3d9066bc51bbcd591257d92f45189da09d16b98c');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 22, N'لغة جديدة:', 'cc9ffbeca4ae6e5231deaab6242500cc1002d3d6aaa57460e5d7a71df06ead71');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 23, N'إنشاء (نسخ من الإسبانية)', 'd86103ba2194b4e17844ec633362b5f878a4f53098c60eb16ac0d3073ee99c65');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 24, N'تعديل / إنشاء ترجمة', '6cabc5f7e2a6515e7efb118040cfcd0a1ecfa1ba54abd6eb8001aa2209be7131');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 25, N'النموذج (Form):', '406507778251fc031b0d4eb2c889d17871434ccce1ca83c604cc3ee329eeccb8');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 26, N'العنصر (Tag منطقي):', '5924a1cd5df90317f74f715dd5eb9a197595514fed9599dc635b1d2735821177');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 27, N'النص المترجم:', '6f03b56ae23546dc04d16f80264e01c470799b2c92be63dd96a33dcd7108c1c0');
INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador) VALUES (7, 28, N'حفظ الترجمة', 'ac58001ecc2ec01cc939402b54e5cedad22b0c329163af7c76706382c5d25a6f');

GO