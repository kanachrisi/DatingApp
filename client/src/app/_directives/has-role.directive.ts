import { Directive, Input, TemplateRef, ViewContainerRef, OnInit } from '@angular/core';
import {take} from 'rxjs/operators';
import { AccountService } from 'src/app/services/account.service';
import {User} from '../_models/user';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit
{
  @Input() appHasRole: string[];
  user: User;

  constructor (private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>, private accountService: AccountService)
  {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user =>
    {
      this.user = user;
    })
  }
  
  ngOnInit(): void
  {
    //..Clear the view if the user has no roles or the user is not logged in
    if (!this.user?.roles || this.user == null)
    {
      this.viewContainerRef.clear();
      return;
    }
    
    //..if the user has some roles 
    if (this.user?.roles.some(r => this.appHasRole.includes(r)))
    {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    }
    else
    {
      this.viewContainerRef.clear();
    }
  }

}
