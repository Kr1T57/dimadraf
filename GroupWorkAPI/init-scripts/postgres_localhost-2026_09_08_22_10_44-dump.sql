--
-- PostgreSQL database dump (Cleaned)
--

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP SCHEMA IF EXISTS asics CASCADE;

CREATE SCHEMA asics;
ALTER SCHEMA asics OWNER TO postgres;

SET default_tablespace = '';
SET default_table_access_method = heap;

CREATE TABLE asics.brands (
    id integer NOT NULL,
    name character varying(15) NOT NULL
);
ALTER TABLE asics.brands OWNER TO postgres;

CREATE TABLE asics.categories (
    id integer NOT NULL,
    name character varying(25) NOT NULL
);
ALTER TABLE asics.categories OWNER TO postgres;

CREATE TABLE asics.colors (
    id integer NOT NULL,
    name character varying(30) NOT NULL
);
ALTER TABLE asics.colors OWNER TO postgres;

CREATE TABLE asics.roles (
    id integer NOT NULL,
    name character varying(45) NOT NULL
);
ALTER TABLE asics.roles OWNER TO postgres;

CREATE TABLE asics.order_statuses (
    id integer NOT NULL,
    name character varying(20) NOT NULL
);
ALTER TABLE asics.order_statuses OWNER TO postgres;

CREATE TABLE asics.users (
    id integer NOT NULL,
    name character varying(30) NOT NULL,
    surname character varying(30) NOT NULL,
    patronymic character varying(35),
    email character varying(50) NOT NULL,
    password character varying(128) NOT NULL,
    phone character varying(12) NOT NULL,
    role_id integer NOT NULL
);
ALTER TABLE asics.users OWNER TO postgres;

CREATE TABLE asics.products (
    id integer NOT NULL,
    brand_id integer NOT NULL,
    category_id integer NOT NULL,
    name character varying(40) NOT NULL,
    description text NOT NULL,
    base_price numeric(8,2) NOT NULL
);
ALTER TABLE asics.products OWNER TO postgres;

CREATE TABLE asics.product_images (
    id integer NOT NULL,
    product_id integer NOT NULL,
    image_url text NOT NULL,
    is_main boolean NOT NULL
);
ALTER TABLE asics.product_images OWNER TO postgres;

CREATE TABLE asics.product_variants (
    id integer NOT NULL,
    product_id integer NOT NULL,
    color_id integer NOT NULL,
    size numeric(5,2) NOT NULL,
    stock_quantity integer NOT NULL
);
ALTER TABLE asics.product_variants OWNER TO postgres;

CREATE TABLE asics.variant_images (
    id integer NOT NULL,
    product_variant_id integer NOT NULL,
    image_path character varying(255) NOT NULL
);
ALTER TABLE asics.variant_images OWNER TO postgres;

CREATE TABLE asics.orders (
    id integer NOT NULL,
    user_id integer NOT NULL,
    order_date timestamp without time zone NOT NULL,
    status_id integer NOT NULL,
    total_amount numeric(8,2) NOT NULL,
    city character varying(25) NOT NULL,
    street character varying(35) NOT NULL,
    house character varying(6) NOT NULL,
    postal_code character varying(6) NOT NULL
);
ALTER TABLE asics.orders OWNER TO postgres;

CREATE TABLE asics.order_items (
    id integer NOT NULL,
    order_id integer NOT NULL,
    product_variant_id integer NOT NULL,
    quantity integer NOT NULL,
    price_at_purchase numeric(8,2) NOT NULL
);
ALTER TABLE asics.order_items OWNER TO postgres;

CREATE TABLE asics.cart_items (
    id integer NOT NULL,
    user_id integer NOT NULL,
    product_variant_id integer NOT NULL,
    quantity integer NOT NULL
);
ALTER TABLE asics.cart_items OWNER TO postgres;

CREATE TABLE asics.product_favorites (
    id integer NOT NULL,
    user_id integer NOT NULL,
    product_id integer NOT NULL
);
ALTER TABLE asics.product_favorites OWNER TO postgres;

CREATE TABLE asics.variant_favorites (
    id integer NOT NULL,
    user_id integer NOT NULL,
    product_variant_id integer NOT NULL
);
ALTER TABLE asics.variant_favorites OWNER TO postgres;

COPY asics.brands (id, name) FROM stdin;
1	ASICS
\.

COPY asics.categories (id, name) FROM stdin;
1	Кроссовки
2	Верх
3	Носки
\.

COPY asics.colors (id, name) FROM stdin;
1	Чёрный
2	Белый
3	Синий
4	Серый
5	Красный
\.

COPY asics.roles (id, name) FROM stdin;
1	Менеджер
2	Клиент
3	Кладовщик
\.

COPY asics.order_statuses (id, name) FROM stdin;
1	Ожидает
2	В обработке
3	Отправлен
4	Доставлен
5	Отменен
\.

COPY asics.users (id, name, surname, patronymic, email, password, phone, role_id) FROM stdin;
1	Alexei	Ivanov	Dmitrievich	manager@asics.ru	AQAAAAIAAYagAAAAEB+tebNek15kHluGxzu4dVO/f/TY0QmdO7IhaWgLBe5DzVR4WYVM7gDlK4eq1MsVVA==	+79161234567	1
2	Maria	Petrova	Sergeevna	maria.petrova@mail.ru	bd94dcda26fccb4e68d6a31f9b5aac0b571ae266d822620e901ef7ebe3a11d4f	+79031234568	2
3	Dmitry	Sidorov	Alexeevich	dima.sid@gmail.com	e8fa823a76f3aaf7068fd2068ba81d1fcb3b680bd854276ddc42d6139754240b	+79261234569	2
4	Anastasia	Kozlova	\N	nastya.koz@yandex.ru	5e13e6a64ccb18aabb63024c4467d3be043ab9d7bc8d3fc11cbc81cb2f2682ec	+79171234570	2
5	Ivan	Novikov	Petrovich	ivan.nov@mail.ru	ec9a7feb2fee607fcaa285bb4a7ab2d1f9d7e2ee81ef799a980085f011169798	+79051234571	2
6	Olga	Fedorova	Ivanovna	olga.fed@gmail.com	7e82e49658f51e4dbe373d3ff9772d7842e91d2d86bbe1502a039e4be3730517	+79161234572	2
7	Sergei	Volkov	Nikolaevich	sergei.v@mail.ru	d499b8162341e1b8dc66cfd06a97b642a8db17fa7b171936a3a5f83eafb2a0db	+79271234573	2
8	Elena	Morozova	\N	elena.mor@yandex.ru	cbbe425102cbcc24ecbb3057b22244792ebcd1b9fca1a6d74f4ff0a76867f5da	+79031234574	2
9	Nikolai	Alekseev	Igorevich	nikolai.a@gmail.com	cf02b9986057bb1a4ff96020e80f6fd9532667beacf42f3812c0f76fd4feb68d	+79151234575	2
10	Tatiana	Lebedeva	Vladimirovna	tatiana.l@mail.ru	ba7518ae6576885c79519e014fafa8e506b637301f7029d3ce26bb311f9596fd	+79061234576	2
16	Петр	Семаков	Иванович	petr@gmailc.com	AQAAAAIAAYagAAAAEODti+sfGg0RE2b6OgY4eAK7X2FaKGEA+7G91NksxWZq473nVxL9sPVaRVGawgoZyw==	83443453455	2
17	Никата	Именов	Тарлокович	nilkata@gmail.com	AQAAAAIAAYagAAAAEPOLyw5b8+QC0d9NaC0aK/sZnXHcEuqZvONeNnKqsrPg9/hjuiU6DtU30b4rgbzmHg==	83432343434	2
18	Рамазан	Иванов	\N	rezervrafael@gmail.com	AQAAAAIAAYagAAAAEJL6b7N70fjqswRJsgcYRnVNsKlWFX6X8M0M1K+ogmH1yQ4AUbyguYdDG8A0hdRFfA==	83433234346	3
19	Дима	Разумченко	\N	dima@gmail.com	AQAAAAIAAYagAAAAEEkg9cdOLSh8U1kxl1GvISMbKtw4ZZZoaiFS2Z3Vo1zkb+YGAvDHjOf81phwKvRvWQ==	87934353345	2
20	Адил	Аитов	Наилевич	adilaitov5@gmail.com	AQAAAAIAAYagAAAAEDvERT4hl7kpiVlsK3LhYeknl2ybXGXJNK+nUliTNtSRbl80xpx7zXODsjm6t8splQ==	89033531256	2
21	Ольга	Фатхулова	Владимировна	olonipko1@gmail.com	AQAAAAIAAYagAAAAEMKBiiH11mJWJpFOAZ8i1K1p9fps5L7cOsOjGcbh6EgLWccnAPGObD90+clT5XblPw==	82335445456	2
22	Тимур	Петров	Иванович	timur_ivanovich@email.com	AQAAAAIAAYagAAAAECmGEx1QNMNXttTTqUWAydr0GH+3WB97UJ+ew3jm18CMHLV+Q+KsxgIiS7K7yLBAzw==	89174561220	2
\.

COPY asics.products (id, brand_id, category_id, name, description, base_price) FROM stdin;
1	1	1	GEL-NIMBUS 26	Премиальные кроссовки для бега на длинные дистанции с максимальной амортизацией FF BLAST+ ECO.	9999.00
2	1	1	GEL-KAYANO 31	Стабилизирующие беговые кроссовки с технологией LITETRUSS для надёжной поддержки стопы.	10499.00
3	1	1	GT-2000 13	Поддерживающие кроссовки для ежедневных тренировок с конструкцией PURERIDE.	7999.00
4	1	1	GEL-CUMULUS 26	Универсальные тренировочные кроссовки с балансом амортизации и отзывчивости.	8499.00
5	1	1	NOVABLAST 4	Энергичные кроссовки с технологией FLYTEFOAM BLAST+ — эффект батута при каждом шаге.	8499.00
6	1	1	GEL-EXCITE 10	Доступные беговые кроссовки с мягкой амортизацией и дышащим верхом из сетки.	4499.00
7	1	1	GEL-1130	Лайфстайл-кроссовки в стиле 90-х с массивной подошвой и премиальным верхом из сетки.	6999.00
8	1	1	GEL-KAYANO 14	Ретро-лайфстайл кроссовки на основе культовой беговой модели 2001 года.	6499.00
9	1	2	Беговая футболка Core	Многофункциональная футболка с технологией ASICS Motion Dry для комфорта во время тренировок.	2499.00
10	1	2	Беговая футболка Road Run	Футболка с короткими рукавами, вентиляционными панелями и светоотражающими элементами.	2999.00
11	1	2	Майка Core Muscle	Лёгкая тренировочная майка с влагоотводящим материалом и свободным кроем.	2199.00
12	1	2	Лонгслив Icon LS	Функциональный лонгслив с термосвойствами для тренировок в прохладную погоду.	3299.00
13	1	2	Лонгслив Winter Run	Тёплый лонгслив с флисовой подкладкой для пробежек и тренировок в холодное время года.	3799.00
14	1	2	Футболка Fujitrail	Техническая беговая футболка из стрейч-ткани с вентиляцией под рукавами.	3299.00
15	1	2	Лёгкая куртка Lite-Show	Высокозаметная куртка для бега со светоотражением 360° и компактной упаковкой.	5999.00
16	1	2	Куртка Accelerate	Ветрозащитная куртка с DWR-пропиткой и карманами на молнии для тренировок в любую погоду.	5499.00
17	1	3	Беговые носки Ultra Comfort	Ультракомфортные беговые носки с усиленной пяткой и носком, зональной компрессией.	699.00
18	1	3	Носки Quick Lyte Cushion	Носки с мягкой подушкой в зоне удара и влагоотводящими свойствами.	599.00
19	1	3	Носки Training	Универсальные тренировочные носки с дополнительной вентиляцией и анатомическим кроем.	499.00
20	1	3	Носки Road+	Компрессионные носки для бега по асфальту с усиленной амортизацией в зоне стопы.	799.00
\.

COPY asics.product_images (id, product_id, image_url, is_main) FROM stdin;
1	1	GEL-NIMBUS 26.png	t
3	3	GT-2000 13.png	t
4	4	GEL-CUMULUS 26.png	t
5	5	NOVABLAST 4.png	t
6	6	GEL-EXCITE 10.png	t
7	7	GEL-1130.png	t
8	8	GEL-KAYANO 14.png	t
9	9	Беговая футболка Core.png	t
10	10	Беговая футболка Road Run.png	t
11	11	Майка Core Muscle.png	t
12	12	Лонгслив Icon LS.png	t
13	13	Лонгслив Winter Run.png	t
14	14	Футболка Fujitrail.png	t
15	15	Лёгкая куртка Lite-Show.png	t
16	16	Куртка Accelerate.png	t
17	17	Беговые носки Ultra Comfort.png	t
18	18	Носки Quick Lyte Cushion.png	t
19	19	Носки Training.png	t
20	20	Носки Road+.png	t
21	2	GEL-KAYANO 31.png	t
\.

COPY asics.product_variants (id, product_id, color_id, size, stock_quantity) FROM stdin;
1	1	1	40.00	30
2	1	2	42.00	17
3	2	1	41.00	18
4	2	4	42.00	15
5	3	3	40.00	20
6	3	1	41.00	12
7	4	4	41.00	19
8	4	2	42.00	17
9	5	2	40.00	17
10	5	5	42.00	12
11	6	1	40.00	31
12	6	3	41.00	26
13	7	2	40.00	20
14	7	4	41.00	18
15	8	4	41.00	19
16	8	2	42.00	16
17	9	1	44.00	38
18	9	2	48.00	34
19	10	3	44.00	30
20	10	1	48.00	16
21	11	1	44.00	42
22	11	4	48.00	37
23	12	1	44.00	26
24	12	3	48.00	22
25	13	4	44.00	32
26	13	1	48.00	27
27	14	3	44.00	25
28	14	4	48.00	20
29	15	1	44.00	20
30	15	5	48.00	14
31	16	3	44.00	18
32	16	1	48.00	14
33	17	1	39.00	60
34	17	2	43.00	55
35	18	1	39.00	70
36	18	4	43.00	65
37	19	2	39.00	80
38	19	1	43.00	75
39	20	1	39.00	50
40	20	5	43.00	44
\.

COPY asics.variant_images (id, product_variant_id, image_path) FROM stdin;
1	1	GEL-KAYANO 31.png
2	2	GEL-KAYANO 31 1.png
3	3	GT-2000 13.png
4	4	GT-2000 13 1.png
5	5	GEL-NIMBUS 26.png
6	6	GEL-NIMBUS 26 1.png
7	7	GEL-CUMULUS 26.png
8	8	GEL-CUMULUS 26 1.png
9	9	GEL-EXCITE 10.png
10	10	GEL-EXCITE 10 1.png
11	11	NOVABLAST 4.png
12	12	NOVABLAST 4 1.png
13	13	GEL-1130.png
14	14	GEL-1130 1.png
15	15	GEL-KAYANO 14.png
16	16	GEL-KAYANO 14 1.png
17	17	Беговая футболка Core.png
18	18	Беговая футболка Core 1.png
19	19	Беговая футболка Road Run.png
20	20	Беговая футболка Road Run 1.png
21	21	Майка Core Muscle.png
22	22	Майка Core Muscle 1.png
23	23	Лонгслив Icon LS.png
24	24	Лонгслив Icon LS 1.png
25	25	Лонгслив Winter Run.png
26	26	Лонгслив Winter Run 1.png
27	27	Футболка Fujitrail.png
28	28	Футболка Fujitrail 1.png
29	29	Лёгкая куртка Lite-Show.png
30	30	Лёгкая куртка Lite-Show 1.png
31	31	Куртка Accelerate.png
32	32	Куртка Accelerate 1.png
33	33	Беговые носки Ultra Comfort.png
34	34	Беговые носки Ultra Comfort 1.png
35	35	Носки Quick Lyte Cushion.png
36	36	Носки Quick Lyte Cushion 1.png
37	37	Носки Training.png
38	38	Носки Training 1.png
39	39	Носки Road+.png
40	40	Носки Road+ 1.png
\.

COPY asics.orders (id, user_id, order_date, status_id, total_amount, city, street, house, postal_code) FROM stdin;
30	18	2026-05-21 11:49:38.410101	1	33496.00	Уфа	Перлова	67	676767
38	18	2026-05-23 23:57:34.123408	1	43694.00	Уфа	петрова	34	897688
39	18	2026-05-24 00:02:59.918765	1	41495.00	Уфа	Чернышева	3	234342
40	18	2026-05-24 21:54:45.929465	1	32095.00	Казань	Пархоменко	45	345453
41	18	2026-05-24 22:16:12.808258	4	31996.00	Иванов	Петрова	45	676643
42	18	2026-05-24 22:33:26.953306	4	15096.00	Петров	Свердлова	54	234535
43	20	2026-05-24 22:41:34.456659	1	41495.00	Уфа	Центральная	20	355365
44	18	2026-05-24 23:47:11.841924	4	29996.00	Казань	Аитова	23	465463
45	21	2026-05-25 08:32:59.003714	4	28496.00	Уфа	Кирово	67	525252
46	18	2026-05-25 08:36:59.755315	1	31496.00	Уфа	Аитова	69	234567
47	18	2026-06-18 18:02:17.110875	1	18996.00	Уфа	Пушкниа	45	234234
\.

COPY asics.order_items (id, order_id, product_variant_id, quantity, price_at_purchase) FROM stdin;
1	30	9	1	8499.00
2	30	6	1	7999.00
3	30	10	2	8499.00
8	38	8	1	8499.00
9	38	7	2	8499.00
10	38	6	1	7999.00
11	38	5	1	7999.00
12	38	21	1	2199.00
13	39	7	1	8499.00
14	39	5	1	7999.00
15	39	6	1	7999.00
16	39	8	2	8499.00
17	40	7	2	8499.00
18	40	8	1	8499.00
19	40	23	2	3299.00
20	41	7	2	8499.00
21	41	8	1	8499.00
22	41	15	1	6499.00
23	42	22	1	2199.00
24	42	21	2	2199.00
25	42	7	1	8499.00
26	43	7	1	8499.00
27	43	8	2	8499.00
28	43	6	1	7999.00
29	43	5	1	7999.00
30	44	9	1	8499.00
31	44	10	2	8499.00
32	44	12	1	4499.00
33	45	11	1	4499.00
34	45	6	3	7999.00
35	46	7	2	8499.00
36	46	8	1	8499.00
37	46	30	1	5999.00
38	47	17	2	2499.00
39	47	13	2	6999.00
\.

COPY asics.cart_items (id, user_id, product_variant_id, quantity) FROM stdin;
6	1	2	3
4	1	20	9
11	22	10	1
12	22	9	1
7	20	11	1
8	20	40	1
\.

COPY asics.product_favorites (id, user_id, product_id) FROM stdin;
1	1	4
2	1	3
4	1	17
11	21	4
12	21	5
13	21	10
15	16	12
16	16	2
24	17	2
25	17	12
26	17	13
27	17	3
28	10	3
30	18	3
31	18	5
32	18	7
33	18	17
34	22	3
35	22	8
36	18	6
\.

COPY asics.variant_favorites (id, user_id, product_variant_id) FROM stdin;
2	18	6
3	18	5
4	18	17
9	20	40
10	20	10
11	20	6
12	16	13
13	16	14
14	16	30
19	17	7
20	17	8
21	17	11
22	17	12
24	1	6
26	18	7
27	18	13
28	18	19
29	18	20
30	18	23
31	21	18
32	22	9
33	22	11
34	22	12
\.

ALTER TABLE ONLY asics.brands
    ADD CONSTRAINT brands_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.categories
    ADD CONSTRAINT categories_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.colors
    ADD CONSTRAINT colors_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.roles
    ADD CONSTRAINT roles_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.order_statuses
    ADD CONSTRAINT order_statuses_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.users
    ADD CONSTRAINT users_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.products
    ADD CONSTRAINT products_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.product_images
    ADD CONSTRAINT product_images_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.product_variants
    ADD CONSTRAINT product_variants_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.variant_images
    ADD CONSTRAINT variant_images_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.orders
    ADD CONSTRAINT orders_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.order_items
    ADD CONSTRAINT order_items_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.cart_items
    ADD CONSTRAINT cart_items_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.product_favorites
    ADD CONSTRAINT product_favorites_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.product_favorites
    ADD CONSTRAINT product_favorites_user_product_uq UNIQUE (user_id, product_id);

ALTER TABLE ONLY asics.variant_favorites
    ADD CONSTRAINT variant_favorites_pk PRIMARY KEY (id);

ALTER TABLE ONLY asics.variant_favorites
    ADD CONSTRAINT variant_favorites_user_variant_uq UNIQUE (user_id, product_variant_id);

ALTER TABLE ONLY asics.users
    ADD CONSTRAINT users_roles_id_fk FOREIGN KEY (role_id) REFERENCES asics.roles(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.products
    ADD CONSTRAINT products_brands_id_fk FOREIGN KEY (brand_id) REFERENCES asics.brands(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.products
    ADD CONSTRAINT products_categories_id_fk FOREIGN KEY (category_id) REFERENCES asics.categories(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.product_images
    ADD CONSTRAINT product_images_products_id_fk FOREIGN KEY (product_id) REFERENCES asics.products(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.product_variants
    ADD CONSTRAINT product_variants_products_id_fk FOREIGN KEY (product_id) REFERENCES asics.products(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.product_variants
    ADD CONSTRAINT product_variants_colors_id_fk FOREIGN KEY (color_id) REFERENCES asics.colors(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.variant_images
    ADD CONSTRAINT variant_images_product_variants_id_fk FOREIGN KEY (product_variant_id) REFERENCES asics.product_variants(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.orders
    ADD CONSTRAINT orders_users_id_fk FOREIGN KEY (user_id) REFERENCES asics.users(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.orders
    ADD CONSTRAINT orders_order_statuses_id_fk FOREIGN KEY (status_id) REFERENCES asics.order_statuses(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.order_items
    ADD CONSTRAINT order_items_orders_id_fk FOREIGN KEY (order_id) REFERENCES asics.orders(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.order_items
    ADD CONSTRAINT order_items_product_variants_id_fk FOREIGN KEY (product_variant_id) REFERENCES asics.product_variants(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.cart_items
    ADD CONSTRAINT cart_items_users_id_fk FOREIGN KEY (user_id) REFERENCES asics.users(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.cart_items
    ADD CONSTRAINT cart_items_product_variants_id_fk FOREIGN KEY (product_variant_id) REFERENCES asics.product_variants(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.product_favorites
    ADD CONSTRAINT product_favorites_users_id_fk FOREIGN KEY (user_id) REFERENCES asics.users(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.product_favorites
    ADD CONSTRAINT product_favorites_products_id_fk FOREIGN KEY (product_id) REFERENCES asics.products(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.variant_favorites
    ADD CONSTRAINT variant_favorites_users_id_fk FOREIGN KEY (user_id) REFERENCES asics.users(id) ON UPDATE CASCADE ON DELETE CASCADE;

ALTER TABLE ONLY asics.variant_favorites
    ADD CONSTRAINT variant_favorites_product_variants_id_fk FOREIGN KEY (product_variant_id) REFERENCES asics.product_variants(id) ON UPDATE CASCADE ON DELETE CASCADE;