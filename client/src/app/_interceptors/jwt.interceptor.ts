import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from './../services/account.service';
import {User} from '../_models/user';
import {take} from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> 
  {
    let currentUser: User;
    
    //By adding pipe(take(1)) will automatically unsubscribe from the observable as soon we have taken 1 item out of it
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => currentUser = user);
    
    if (currentUser)
    {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`
        }
      });
      
    }
    return next.handle(request);
  }
}
