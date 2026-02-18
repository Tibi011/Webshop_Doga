const express = require('express');
const app = express();
const mysql = require('mysql2');

const PORT = 3000;
const host = 'localhost'; 

// Beállítjuk a JSON-ben érkező adatok kezelését
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

//kapcsolat beállítása az adatbázissal 
const conn = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: '',
    database: 'mini_webshop',
    timezone: 'Z'
})

// Alap route
app.get('/', (req, res) => {
  res.send('Node.js szerver működik!');
});

// összes termék lekérdezése
app.get('/products', (req, res) => {
    conn.query('SELECT * FROM products', (err, result) => {
        if (err) {
            console.log(err);
            return res.status(500).json({ error: 'Hiba a lekérdezés során!' });
        }

        if (result.length === 0) {
            return res.status(404).json({ error: 'Nem található!' });
        }

        return res.status(200).json(result);
    });
});





app.get('/categories', (req, res) => {

    const sql = `
        SELECT DISTINCT Category
        FROM products
        ORDER BY Category
    `;

    conn.query(sql, (err, result) => {

        if (err)
        {
            console.log(err);
            return res.status(500).json(err);
        }

        res.json(result);

    });

});



app.get('/products/filter', (req, res) => {

    let category = req.query.category;
    let inStock = req.query.inStock;

    let sql = `
        SELECT 
            Id,
            Name,
            Price,
            Category,
            inStock
        FROM products
        WHERE 1=1
    `;

    if (category)
    {
        sql += ` AND Category = '${category}'`;
    }

    if (inStock === "true")
    {
        sql += ` AND inStock > 0`;
    }

    conn.query(sql, (err, result) => {

        if (err)
        {
            console.log(err);
            return res.status(500).json(err);
        }

        res.json(result);

    });

});



app.post('/products', (req, res) => {

    const sql = `
        INSERT INTO products
        (Name, Price, Category, inStock)
        VALUES (?, ?, ?, ?)
    `;

    conn.query(sql, [

        req.body.Name,
        req.body.Price,
        req.body.Category,
        req.body.inStock

    ], (err, result) => {

        if (err)
        {
            console.log(err);
            return res.status(500).json(err);
        }

        res.json({
            message: "Termék hozzáadva",
            Id: result.insertId
        });

    });

});



app.put('/products/:id', (req, res) => {

    const sql = `
        UPDATE products
        SET
            Name = ?,
            Price = ?,
            Category = ?,
            inStock = ?
        WHERE Id = ?
    `;

    conn.query(sql, [

        req.body.Name,
        req.body.Price,
        req.body.Category,
        req.body.inStock,
        req.params.id

    ], (err, result) => {

        if (err)
        {
            console.log(err);
            return res.status(500).json(err);
        }

        res.json({
            message: "Termék módosítva"
        });

    });

});



app.delete('/products/:id', (req, res) => {

    const sql = `
        DELETE FROM products
        WHERE Id = ?
    `;

    conn.query(sql, [req.params.id], (err, result) => {

        if (err)
        {
            console.log(err);
            return res.status(500).json(err);
        }

        res.json({
            message: "Termék törölve"
        });

    });

});






// Szerver indítása
app.listen(PORT, () => {
    console.log(`Szerver fut: http://${host}:${PORT}`);
  });