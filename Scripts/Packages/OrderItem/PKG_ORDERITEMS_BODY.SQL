create or replace NONEDITIONABLE PACKAGE BODY PKG_ORDERITEMS AS

  PROCEDURE GetOrderItemById(
    p_order_id     IN ORDERS_ITEMS.ORDER_ID%TYPE,
    p_line_item_id IN ORDERS_ITEMS.LINE_ITEM_ID%TYPE,
    p_cursor       OUT SYS_REFCURSOR
  ) IS
  BEGIN
    OPEN p_cursor FOR
      SELECT ORDER_ID,
             LINE_ITEM_ID,
             PRODUCT_ID,
             UNIT_PRICE,
             QUANTITY,
             SHIPMENT_ID
      FROM ORDERS_ITEMS
      WHERE ORDER_ID = p_order_id
        AND LINE_ITEM_ID = p_line_item_id;

  EXCEPTION
    WHEN OTHERS THEN
      RAISE_APPLICATION_ERROR(-21010, 'Error en GetOrderItemById: ' || SQLERRM);
  END GetOrderItemById;

  PROCEDURE GetAllOrderItems(
    p_cursor OUT SYS_REFCURSOR
  ) IS
  BEGIN
    OPEN p_cursor FOR
      SELECT ORDER_ID,
             LINE_ITEM_ID,
             PRODUCT_ID,
             UNIT_PRICE,
             QUANTITY,
             SHIPMENT_ID
      FROM ORDERS_ITEMS
      ORDER BY ORDER_ID, LINE_ITEM_ID;

  EXCEPTION
    WHEN OTHERS THEN
      RAISE_APPLICATION_ERROR(-21020, 'Error en GetAllOrderItems: ' || SQLERRM);
  END GetAllOrderItems;

  PROCEDURE AddOrderItem(
    p_order_id      IN ORDERS_ITEMS.ORDER_ID%TYPE,
    p_line_item_id  IN ORDERS_ITEMS.LINE_ITEM_ID%TYPE,
    p_product_id    IN ORDERS_ITEMS.PRODUCT_ID%TYPE,
    p_unit_price    IN ORDERS_ITEMS.UNIT_PRICE%TYPE,
    p_quantity      IN ORDERS_ITEMS.QUANTITY%TYPE,
    p_shipment_id   IN ORDERS_ITEMS.SHIPMENT_ID%TYPE,
    p_rows_inserted OUT NUMBER
  ) IS
  BEGIN
    -- Validaciones básicas de parámetros obligatorios
    IF p_order_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-21030, 'ORDER_ID es requerido');
    END IF;
    IF p_line_item_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-21031, 'LINE_ITEM_ID es requerido');
    END IF;
    IF p_product_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-21032, 'PRODUCT_ID es requerido');
    END IF;
    IF p_unit_price IS NULL THEN
      RAISE_APPLICATION_ERROR(-21033, 'UNIT_PRICE es requerido');
    END IF;
    IF p_quantity IS NULL THEN
      RAISE_APPLICATION_ERROR(-21034, 'QUANTITY es requerido');
    END IF;

    BEGIN
      INSERT INTO ORDERS_ITEMS (
        ORDER_ID,
        LINE_ITEM_ID,
        PRODUCT_ID,
        UNIT_PRICE,
        QUANTITY,
        SHIPMENT_ID
      )
      VALUES (
        p_order_id,
        p_line_item_id,
        p_product_id,
        p_unit_price,
        p_quantity,
        p_shipment_id
      );

      p_rows_inserted := SQL%ROWCOUNT;

    EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
         RAISE_APPLICATION_ERROR(-21035, 'Elemento duplicado en ORDERS_ITEMS');
      WHEN OTHERS THEN
         RAISE_APPLICATION_ERROR(-21036, 'Error al añadir OrderItem: ' || SQLERRM);
    END;
  END AddOrderItem;

  PROCEDURE UpdateOrderItem(
    p_order_id      IN ORDERS_ITEMS.ORDER_ID%TYPE,
    p_line_item_id  IN ORDERS_ITEMS.LINE_ITEM_ID%TYPE,
    p_product_id    IN ORDERS_ITEMS.PRODUCT_ID%TYPE,
    p_unit_price    IN ORDERS_ITEMS.UNIT_PRICE%TYPE,
    p_quantity      IN ORDERS_ITEMS.QUANTITY%TYPE,
    p_shipment_id   IN ORDERS_ITEMS.SHIPMENT_ID%TYPE,
    p_rows_updated  OUT NUMBER
  ) IS
    v_exists NUMBER;
  BEGIN
    IF p_order_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-21040, 'ORDER_ID es requerido');
    END IF;
    IF p_line_item_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-21041, 'LINE_ITEM_ID es requerido');
    END IF;

    SELECT COUNT(*)
      INTO v_exists
      FROM ORDERS_ITEMS
     WHERE ORDER_ID = p_order_id
       AND LINE_ITEM_ID = p_line_item_id;

    IF v_exists = 0 THEN
      RAISE_APPLICATION_ERROR(-21042, 'Registro no encontrado para ORDER_ID ' || p_order_id || ' y LINE_ITEM_ID ' || p_line_item_id);
    END IF;

    BEGIN
      UPDATE ORDERS_ITEMS
         SET PRODUCT_ID  = p_product_id,
             UNIT_PRICE  = p_unit_price,
             QUANTITY    = p_quantity,
             SHIPMENT_ID = p_shipment_id
       WHERE ORDER_ID    = p_order_id
         AND LINE_ITEM_ID = p_line_item_id;

      p_rows_updated := SQL%ROWCOUNT;

    EXCEPTION
      WHEN OTHERS THEN
         RAISE_APPLICATION_ERROR(-21043, 'Error al actualizar OrderItem: ' || SQLERRM);
    END;

  END UpdateOrderItem;

  PROCEDURE DeleteOrderItem(
    p_order_id      IN ORDERS_ITEMS.ORDER_ID%TYPE,
    p_line_item_id  IN ORDERS_ITEMS.LINE_ITEM_ID%TYPE,
    p_rows_deleted  OUT NUMBER
  ) IS
    v_exists NUMBER;
  BEGIN
    IF p_order_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-21050, 'ORDER_ID es requerido');
    END IF;
    IF p_line_item_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-21051, 'LINE_ITEM_ID es requerido');
    END IF;

    SELECT COUNT(*)
      INTO v_exists
      FROM ORDERS_ITEMS
     WHERE ORDER_ID = p_order_id
       AND LINE_ITEM_ID = p_line_item_id;

    IF v_exists = 0 THEN
      RAISE_APPLICATION_ERROR(-21052, 'Registro no encontrado para ORDER_ID ' || p_order_id || ' y LINE_ITEM_ID ' || p_line_item_id);
    END IF;

    BEGIN
      DELETE FROM ORDERS_ITEMS
       WHERE ORDER_ID = p_order_id
         AND LINE_ITEM_ID = p_line_item_id;

      p_rows_deleted := SQL%ROWCOUNT;
      COMMIT;

    EXCEPTION
      WHEN OTHERS THEN
         ROLLBACK;
         RAISE_APPLICATION_ERROR(-21053, 'Error al eliminar OrderItem: ' || SQLERRM);
    END;

  END DeleteOrderItem;

END PKG_ORDERITEMS;
