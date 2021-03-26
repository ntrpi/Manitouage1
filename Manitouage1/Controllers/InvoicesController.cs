﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Manitouage1.Models;
using Manitouage1.Models.ViewModels;

namespace Manitouage1.Controllers
{
    public class InvoicesController: Controller
    {
        private static readonly ControllersHelper helper;

        static InvoicesController()
        {
            helper = new ControllersHelper( "Invoice" );
        }

        // A utility function to save a few characters when using the ControllersHelper to construct a url.
        private string getUrl( string action, int id = 0 )
        {
            return helper.getUrl( action, id );
        }

        /// <summary>
        /// Get and display a list of all the Invoices in the database.
        /// </summary>
        /// <returns>A View containing a List of InvoiceDto objects.</returns>
        /// <example>
        /// GET: Invoices
        /// </example>
        public ActionResult Index()
        {
            HttpResponseMessage response = helper.doGetRequest( getUrl( "Get" ) + "s" );
            if( !response.IsSuccessStatusCode ) {
                return View();
            }

            return View( helper.getFromResponse<IEnumerable<InvoiceDto>>( response ) );
        }

        // A utility function that facilitates the call to the helper to retrieve a Invoice from the database.
        private InvoiceDto getInvoiceDto( int invoiceId )
        {
            HttpResponseMessage response = helper.doGetRequest( getUrl( "Get", invoiceId ) );
            if( !response.IsSuccessStatusCode ) {
                return null;
            }
            return helper.getFromResponse<InvoiceDto>( response );
        }

        /// <summary>
        /// Get and display the information for the Invoice with the given ID.
        /// </summary>
        /// <param name="id">The ID of the Invoice in the database to retrieve.</param>
        /// <returns>A View containing a InvoiceDto object if it was successfully retrieved, otherwise it contains null.</returns>
        /// <example>
        /// GET: Invoices/Details/5
        /// </example>
        public ActionResult Details( int id )
        {
            InvoiceDto invoiceDto = getInvoiceDto( id );

            // Get the product IDs.
            string productXInvoicesUrl = ControllersHelper.getUrl( "ProductXInvoice", "Get", 0 );
            productXInvoicesUrl += "sForInvoice/" + id;
            IEnumerable<ProductXInvoiceDto> productXInvoiceDtos = helper.doGetAndGetFromResponse<List<ProductXInvoiceDto>>( productXInvoicesUrl );

            // Get the products.
            string productsUrl = ControllersHelper.getUrl( "Product", "Get", 0 );
            List<ProductDto> productDtos = new List<ProductDto>();
            foreach( ProductXInvoiceDto productXInvoice in productXInvoiceDtos ) {
                productDtos.Add( 
                    helper.doGetAndGetFromResponse<ProductDto>( 
                        productsUrl + "/" + productXInvoice.productId ) );
            }

            ViewInvoice viewInvoice = new ViewInvoice {
                invoiceDto = invoiceDto,
                productDtos = productDtos
            };

            return View( viewInvoice );
        }

        /// <summary>
        /// Display the form to create a new Invoice.
        /// </summary>
        /// <returns>A View with the relevant input fields to create a new Invoice.</returns>
        /// <example>
        /// GET: Invoices/Create
        /// </example>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create a new record in the database with the information contained in the Invoice object.
        /// </summary>
        /// <param name="invoice">A Invoice object, received as form data.</param>
        /// <returns>If the new Invoice was created, a RedirectToAction pointing to Details and containing the new Invoice's ID is returned, otherwise, a View with an error message in the ViewBag.</returns>
        /// <example>
        /// POST: Invoices/Create
        /// Form Data: Invoice
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Invoice invoice )
        {
            HttpResponseMessage response = helper.doPostRequest( getUrl( "Create" ), invoice );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to add invoice.";
                return View();
            }

            InvoiceDto invoiceDto = helper.getFromResponse<InvoiceDto>( response );
            return RedirectToAction( "Details", new {
                id = invoiceDto.invoiceId
            } );
        }

        /// <summary>
        /// Display the form to make changes to a Invoice.
        /// </summary>
        /// <param name="id">The ID of the Invoice to update.</param>
        /// <returns>A View with the relevant input fields to edit a Invoice.</returns>
        /// <example>
        /// GET: Invoices/Edit/5
        /// </example>
        public ActionResult Edit( int id )
        {
            return View( getInvoiceDto( id ) );
        }

        /// <summary>
        /// Update the record in the database with the information contained in the Invoice object.
        /// </summary>
        /// <param name="id">The ID of the invoice to update.</param>
        /// <param name="invoice">A Invoice object, received as form data.</param>
        /// <returns>If the new Invoice was created, a RedirectToAction pointing to Details and containing the new Invoice's ID is returned, otherwise, a View with an error message in the ViewBag.</returns>
        /// <example>
        /// POST: Invoices/Create
        /// Form Data: Invoice
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( int id, Invoice invoice )
        {
            HttpResponseMessage response = helper.doPostRequest( getUrl( "Update", id ), invoice );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to add invoice.";
                return View();
            }

            return RedirectToAction( "Details", new {
                id = id
            } );
        }

        /// <summary>
        /// Display a view that shows the details of the Invoice and asks the user to confirm its deletion.
        /// </summary>
        /// <param name="id">The ID of the Invoice to delete.</param>
        /// <returns>A View containing a InvoiceDto if retrieved successfully from the database, otherwise it is empty.</returns>
        /// <example>
        /// GET: Invoices/DeleteConfirm/5
        /// </example>
        [HttpGet]
        public ActionResult DeleteConfirm( int id )
        {
            return View( getInvoiceDto( id ) );
        }

        /// <summary>
        /// Delete the record in the database with the given ID.
        /// </summary>
        /// <param name="id">The ID of the invoice to update.</param>
        /// <param name="collection">A collection of properties passed in from the caller.</param>
        /// <returns>An ActionResult that Redirects to Index.</returns>
        /// <example>
        /// POST: Invoices/Create
        /// Form Data: Invoice
        /// </example>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete( int id, FormCollection collection )
        {
            // TODO: do something with the response.
            helper.doPostRequest( getUrl( "Delete", id ), "" );
            return RedirectToAction( "Index" );
        }
    }
}