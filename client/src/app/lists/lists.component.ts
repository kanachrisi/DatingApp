import { Component, OnInit } from '@angular/core';
import {Member} from '../_models/member';
import { MembersService } from 'src/app/services/members.service';
import { Pagination } from './../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  
  members: Partial<Member[]>; //This means each one property of member is gonna be optional
  predicate: string = 'liked';
  pageNumber: number = 1;
  pageSize = 5;
  pagination: Pagination
  
  
  constructor(private memberService : MembersService) { }

  ngOnInit(): void
  {
    this.loadLikes();
  }
  
  loadLikes()
  {
    /* this.memberService.getLikes(this.predicate).subscribe((response) =>
    {
      this.members = response;
    }) */
    this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe(response =>
    {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }
  
  pageChanged(event: any)
  {
    this.pageNumber = event.page;
    this.loadLikes();
  }

}
