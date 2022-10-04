import { Component, Input, OnInit } from '@angular/core';
import {Member} from 'src/app/_models/member';
import { MembersService } from 'src/app/services/members.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit 
{
  @Input() member: Member
 
  
  constructor(private memberSevice: MembersService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }
  
  addLike(member: Member)
  {
    this.memberSevice.addLike(member.username).subscribe(() =>
    {
      
      this.toastr.success('You have liked ' + member.knownAs);
    })
  }

}
