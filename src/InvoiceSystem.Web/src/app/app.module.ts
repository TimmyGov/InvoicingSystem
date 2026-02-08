import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { InvoiceListComponent } from './components/invoice-list/invoice-list.component';
import { CreateInvoiceComponent } from './components/create-invoice/create-invoice.component';
import { InvoiceDetailComponent } from './components/invoice-detail/invoice-detail.component';
import { InvoiceService } from './services/invoice.service';

const routes: Routes = [
  { path: '', redirectTo: '/invoices', pathMatch: 'full' },
  { path: 'invoices', component: InvoiceListComponent },
  { path: 'invoices/create', component: CreateInvoiceComponent },
  { path: 'invoices/:id', component: InvoiceDetailComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    InvoiceListComponent,
    CreateInvoiceComponent,
    InvoiceDetailComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(routes)
  ],
  providers: [InvoiceService],
  bootstrap: [AppComponent]
})
export class AppModule { }
