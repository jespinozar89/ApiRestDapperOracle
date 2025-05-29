create or replace NONEDITIONABLE PACKAGE PKG_ORDERITEMS IS

  -- Obtiene un item de orden específico (clave compuesta: ORDER_ID y LINE_ITEM_ID)
  PROCEDURE GetOrderItemById(
    p_order_id     IN  ORDERS_ITEMS.ORDER_ID%TYPE,
    p_line_item_id IN  ORDERS_ITEMS.LINE_ITEM_ID%TYPE,
    p_cursor       OUT SYS_REFCURSOR
  );

  -- Obtiene todos los items de orden
  PROCEDURE GetAllOrderItems(
    p_cursor OUT SYS_REFCURSOR
  );

  -- Agrega un nuevo registro en ORDERS_ITEMS
  PROCEDURE AddOrderItem(
    p_order_id      IN ORDERS_ITEMS.ORDER_ID%TYPE,
    p_line_item_id  IN ORDERS_ITEMS.LINE_ITEM_ID%TYPE,
    p_product_id    IN ORDERS_ITEMS.PRODUCT_ID%TYPE,
    p_unit_price    IN ORDERS_ITEMS.UNIT_PRICE%TYPE,
    p_quantity      IN ORDERS_ITEMS.QUANTITY%TYPE,
    p_shipment_id   IN ORDERS_ITEMS.SHIPMENT_ID%TYPE,
    p_rows_inserted OUT NUMBER
  );

  -- Actualiza un registro existente en ORDERS_ITEMS
  PROCEDURE UpdateOrderItem(
    p_order_id      IN ORDERS_ITEMS.ORDER_ID%TYPE,
    p_line_item_id  IN ORDERS_ITEMS.LINE_ITEM_ID%TYPE,
    p_product_id    IN ORDERS_ITEMS.PRODUCT_ID%TYPE,
    p_unit_price    IN ORDERS_ITEMS.UNIT_PRICE%TYPE,
    p_quantity      IN ORDERS_ITEMS.QUANTITY%TYPE,
    p_shipment_id   IN ORDERS_ITEMS.SHIPMENT_ID%TYPE,
    p_rows_updated  OUT NUMBER
  );

  -- Elimina un registro de ORDERS_ITEMS
  PROCEDURE DeleteOrderItem(
    p_order_id      IN ORDERS_ITEMS.ORDER_ID%TYPE,
    p_line_item_id  IN ORDERS_ITEMS.LINE_ITEM_ID%TYPE,
    p_rows_deleted  OUT NUMBER
  );

END PKG_ORDERITEMS;
