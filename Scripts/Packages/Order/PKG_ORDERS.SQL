create or replace NONEDITIONABLE PACKAGE PKG_ORDERS AS 

 -- Buscar orden por ID
  PROCEDURE GetOrderById(
    p_order_id   IN  ORDERS.ORDER_ID%TYPE,
    p_cursor     OUT SYS_REFCURSOR
  );
  
  -- Obtener todas las órdenes
  PROCEDURE GetAllOrders(
    p_cursor OUT SYS_REFCURSOR
  );
  
  PROCEDURE AddOrder(
    p_order_tms    IN  ORDERS.ORDER_TMS%TYPE,
    p_customer_id  IN  ORDERS.CUSTOMER_ID%TYPE,
    p_order_status IN  ORDERS.ORDER_STATUS%TYPE,
    p_store_id     IN  ORDERS.STORE_ID%TYPE,
    p_order_id     OUT ORDERS.ORDER_ID%TYPE
  );
  
  PROCEDURE UpdateOrder(
    p_order_id      IN  ORDERS.ORDER_ID%TYPE,
    p_order_tms     IN  ORDERS.ORDER_TMS%TYPE,
    p_customer_id   IN  ORDERS.CUSTOMER_ID%TYPE,
    p_order_status  IN  ORDERS.ORDER_STATUS%TYPE,
    p_store_id      IN  ORDERS.STORE_ID%TYPE,
    p_rows_updated  OUT NUMBER
  );
  
  PROCEDURE DeleteOrder(
    p_order_id     IN  ORDERS.ORDER_ID%TYPE,
    p_rows_deleted OUT NUMBER
  );
  
END PKG_ORDERS;