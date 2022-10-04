import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import {NavigationExtras, Router} from '@angular/router';
import {catchError} from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private route: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error =>
      {
        //HttpErrorResponse
        if (error)
        {
          switch (error.status) 
          {
            case 400:
              if (error.error.errors)
              {
                const my_errors = error.error.errors;
                const modalStateErrors = [];
                for (const key in my_errors)
                {
                  if (my_errors[key])
                  {
                    modalStateErrors.push(my_errors[key]);
                  }
                  
                }
                throw modalStateErrors.flat();
              }
              
              else if(typeof(error.error) === 'object')
              {
                this.toastr.error(error.error.title, error.error.status);
              }
              else
              {
                this.toastr.error(error.error, error.status);
              }
              break;
            
            case 401:
              this.toastr.error("Unauthorized", error.status);
              break;
            
            case 404:
              this.route.navigateByUrl('/not-found');
              break;
            
            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}};
              this.route.navigateByUrl('/server-error', navigationExtras);
              break;
            
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        return throwError(error);
      })
    )
  }
}
