create or replace NONEDITIONABLE PACKAGE BODY PKG_CUSTOMERS AS

  --Buscar cliente por id
  PROCEDURE GetByIdCustomer(
    p_customer_id IN CUSTOMERS.CUSTOMER_ID%TYPE,
    p_cursor OUT SYS_REFCURSOR
  ) IS
  BEGIN
    OPEN p_cursor FOR
      SELECT CUSTOMER_ID, EMAIL_ADDRESS, FULL_NAME
      FROM CUSTOMERS
      WHERE CUSTOMER_ID = p_customer_id;
  EXCEPTION
    WHEN OTHERS THEN
      -- Manejo de errores básico
      RAISE_APPLICATION_ERROR(-20001, 'Error en GetByIdCustomer: ' || SQLERRM);
  END GetByIdCustomer;
 
  --Obtener todos los clientes
  PROCEDURE GetAllCustomers(
    p_cursor OUT SYS_REFCURSOR
  ) IS
  BEGIN
    OPEN p_cursor FOR
      SELECT CUSTOMER_ID, EMAIL_ADDRESS, FULL_NAME
      FROM CUSTOMERS
      ORDER BY CUSTOMER_ID;
  EXCEPTION
    WHEN OTHERS THEN
      -- Manejo de errores básico
      RAISE_APPLICATION_ERROR(-20002, 'Error en GetAllCustomers: ' || SQLERRM);
  END GetAllCustomers;
  
    --Añadir un cliente
    PROCEDURE AddCustomer(
    p_email_address IN CUSTOMERS.EMAIL_ADDRESS%TYPE,
    p_full_name IN CUSTOMERS.FULL_NAME%TYPE,
    p_customer_id OUT CUSTOMERS.CUSTOMER_ID%TYPE
  ) IS
  BEGIN
    -- Validación básica
    IF p_email_address IS NULL OR p_full_name IS NULL THEN
      RAISE_APPLICATION_ERROR(-20001, 'Email y nombre son requeridos');
    END IF;
    
    -- Insertar con manejo de errores
    BEGIN
      INSERT INTO CUSTOMERS (
        EMAIL_ADDRESS,
        FULL_NAME
      ) VALUES (
        p_email_address,
        p_full_name
      )
      RETURNING CUSTOMER_ID INTO p_customer_id;
    EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        RAISE_APPLICATION_ERROR(-20002, 'El email ya está registrado');
      WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(-20003, 'Error al agregar cliente: ' || SQLERRM);
    END;
  END AddCustomer;
  
  --Actualizar un cliente
  PROCEDURE UpdateCustomer(
    p_customer_id IN CUSTOMERS.CUSTOMER_ID%TYPE,
    p_email_address IN CUSTOMERS.EMAIL_ADDRESS%TYPE,
    p_full_name IN CUSTOMERS.FULL_NAME%TYPE,
    p_rows_updated OUT NUMBER
  ) IS
  BEGIN
    -- Validación básica de parámetros
    IF p_email_address IS NULL OR p_full_name IS NULL THEN
      RAISE_APPLICATION_ERROR(-20001, 'Email y nombre son requeridos');
    END IF;
    
    -- Verificar que el cliente existe
    SELECT COUNT(*)
    INTO p_rows_updated
    FROM CUSTOMERS
    WHERE CUSTOMER_ID = p_customer_id;
    
    IF p_rows_updated = 0 THEN
      RAISE_APPLICATION_ERROR(-20004, 'Cliente no encontrado con ID: ' || p_customer_id);
    END IF;
    
    -- Actualizar el registro
    BEGIN
      UPDATE CUSTOMERS SET
        EMAIL_ADDRESS = p_email_address,
        FULL_NAME = p_full_name
      WHERE CUSTOMER_ID = p_customer_id;
      
      p_rows_updated := SQL%ROWCOUNT;
      
    EXCEPTION
      WHEN OTHERS THEN
        RAISE_APPLICATION_ERROR(-20003, 'Error al actualizar cliente: ' || SQLERRM);
    END;
  END UpdateCustomer;
  
  --Eliminar un cliente
   PROCEDURE DeleteCustomer(
    p_customer_id IN CUSTOMERS.CUSTOMER_ID%TYPE,
    p_rows_deleted OUT NUMBER
  ) IS
    v_customer_exists NUMBER;
  BEGIN
    -- Validación básica de parámetros
    IF p_customer_id IS NULL THEN
      RAISE_APPLICATION_ERROR(-20001, 'ID de cliente es requerido');
    END IF;
    
    -- Verificar que el cliente existe y obtener sus datos
    BEGIN
      SELECT COUNT(*)
      INTO v_customer_exists
      FROM CUSTOMERS
      WHERE CUSTOMER_ID = p_customer_id;
      
      IF v_customer_exists = 0 THEN
        RAISE_APPLICATION_ERROR(-20004, 'Cliente no encontrado con ID: ' || p_customer_id);
      END IF;
      
      -- Eliminar el registro
      DELETE FROM CUSTOMERS
      WHERE CUSTOMER_ID = p_customer_id;
      
      p_rows_deleted := SQL%ROWCOUNT;
      
      -- Confirmar la transacción explícitamente
      COMMIT;
      
    EXCEPTION
      WHEN OTHERS THEN
        ROLLBACK;
        RAISE_APPLICATION_ERROR(-20005, 'Error al eliminar cliente: ' || SQLERRM);
    END;
  END DeleteCustomer;

END PKG_CUSTOMERS;