import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if (error) {
          switch (error.status) {
            case 400:
              if (Array.isArray(error.error)) {
                const modalStateErrors = [];
                for (const key in error.error) {
                  const err = error.error[key];
                  modalStateErrors.push(err.description);
                }
                throw modalStateErrors.flat();
              }
              if (error.error.errors) {
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (Object.prototype.hasOwnProperty.call(error.error.errors, key)) {
                    const err = error.error.errors[key];
                    modalStateErrors.push(err);
                  }
                }
                throw modalStateErrors.flat();
              } else {
                this.toastr.error(error.error, error.status + ' ' + error.statusText);
              }
              break;
            case 401:
              this.toastr.error(error.statusText, error.status);
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = { state: { error: error.error } };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong!');
              break;
          }
        }
        return throwError(error);
      })
    )
  }
}
