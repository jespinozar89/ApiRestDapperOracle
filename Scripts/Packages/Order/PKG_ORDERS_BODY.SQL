create or replace NONEDITIONABLE PACKAGE BODY PKG_ORDERS AS

  PROCEDURE GetOrderById(
    p_order_id   IN  ORDERS.ORDER_ID%TYPE,
    p_cursor     OUT SYS_REFCURSOR
  ) IS
  BEGIN
    OPEN p_cursor FOR
      SELECT ORDER_ID,
             ORDER_TMS,
             CUSTOMER_ID,
             ORDER_STATUS,
             STORE_ID
      FROM ORDERS
      WHERE ORDER_ID = p_order_id;

  EXCEPTION
    WHEN OTHERS THEN
      RAISE_APPLICATION_ERROR(-20010, 'Error en GetOrderById: ' || SQLERRM);
  END GetOrderById;
  
  PROCEDURE GetAllOrders(
    p_cursor OUT SYS_REFCURSOR
  ) IS
  BEGIN
    OPEN p_cursor FOR
      SELECT ORDER_ID,
             ORDER_TMS,
             CUSTOMER_ID,
             ORDER_STATUS,
             STORE_ID
      FROM ORDERS
      ORDER BY ORDER_ID DESC;  -- Orden descendente por fecha de creación

  EXCEPTION
    WHEN OTHERS THEN
      RAISE_APPLICATION_ERROR(-20020, 'Error en GetAllOrders: ' || SQLERRM);
  END GetAllOrders;
  
  PROCEDURE AddOrder(
    p_order_tms    IN  ORDERS.ORDER_TMS%TYPE,
    p_customer_id  IN  ORDERS.CUSTOMER_ID%TYPE,
    p_order_status IN  ORDERS.ORDER_STATUS%TYPE,
    p_store_id     IN  ORDERS.STORE_ID%TYPE,
    p_order_id     OUT ORDERS.ORDER_ID%TYPE
  ) IS
  BEGIN
    -- Validación de parámetros obligatorios
    IF p_customer_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-20030, 'ID de cliente es requerido');
    END IF;

    IF p_order_status IS NULL THEN
      RAISE_APPLICATION_ERROR(-20031, 'Estado de la orden es requerido');
    END IF;

    IF p_store_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-20032, 'ID de tienda es requerido');
    END IF;

    -- Insertar con manejo de errores
    BEGIN
      INSERT INTO ORDERS (
        ORDER_TMS,
        CUSTOMER_ID,
        ORDER_STATUS,
        STORE_ID
      ) VALUES (
        NVL(p_order_tms, SYSDATE),  -- Valor por defecto si es null
        p_customer_id,
        p_order_status,
        p_store_id
      )
      RETURNING ORDER_ID INTO p_order_id;

    EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        RAISE_APPLICATION_ERROR(-20033, 'Orden duplicada');
      WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(-20034, 'Error al crear orden: ' || SQLERRM);
    END;

  END AddOrder;
  
  PROCEDURE UpdateOrder(
    p_order_id      IN  ORDERS.ORDER_ID%TYPE,
    p_order_tms     IN  ORDERS.ORDER_TMS%TYPE,
    p_customer_id   IN  ORDERS.CUSTOMER_ID%TYPE,
    p_order_status  IN  ORDERS.ORDER_STATUS%TYPE,
    p_store_id      IN  ORDERS.STORE_ID%TYPE,
    p_rows_updated  OUT NUMBER
  ) IS
    v_order_exists NUMBER;
  BEGIN
    -- Validación de parámetros obligatorios
    IF p_customer_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-20040, 'ID de cliente es requerido');
    END IF;

    -- Verificar existencia de la orden
    SELECT COUNT(*)
    INTO v_order_exists
    FROM ORDERS
    WHERE ORDER_ID = p_order_id;

    IF v_order_exists = 0 THEN
      RAISE_APPLICATION_ERROR(-20043, 'Orden no encontrada con ID: ' || p_order_id);
    END IF;

    -- Actualizar registro
    BEGIN
      UPDATE ORDERS SET
        ORDER_TMS = NVL(p_order_tms, ORDER_TMS), -- Mantiene valor actual si es NULL
        CUSTOMER_ID = p_customer_id,
        ORDER_STATUS = p_order_status,
        STORE_ID = p_store_id
      WHERE ORDER_ID = p_order_id;

      p_rows_updated := SQL%ROWCOUNT;

    EXCEPTION
      WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(-20044, 'Error al actualizar orden: ' || SQLERRM);
    END;

  END UpdateOrder;

  PROCEDURE DeleteOrder(
    p_order_id     IN  ORDERS.ORDER_ID%TYPE,
    p_rows_deleted OUT NUMBER
  ) IS
    v_order_exists NUMBER;
  BEGIN
    -- Validación básica de parámetros
    IF p_order_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-20050, 'ID de orden es requerido');
    END IF;

    -- Verificar existencia de la orden
    SELECT COUNT(*)
    INTO v_order_exists
    FROM ORDERS
    WHERE ORDER_ID = p_order_id;

    IF v_order_exists = 0 THEN
      RAISE_APPLICATION_ERROR(-20051, 'Orden no encontrada con ID: ' || p_order_id);
    END IF;

    -- Eliminar el registro
    BEGIN
      DELETE FROM ORDERS
      WHERE ORDER_ID = p_order_id;

      p_rows_deleted := SQL%ROWCOUNT;
      COMMIT;

    EXCEPTION
      WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20052, 'Error al eliminar orden: ' || SQLERRM);
    END;

  END DeleteOrder;


END PKG_ORDERS;