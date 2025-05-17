import {Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn} from 'typeorm';
import {Player} from './Player';

@Entity()
export class UnlockedCharacter {

  @PrimaryGeneratedColumn()
  id!: number;

  @ManyToOne(() => Player)
  @JoinColumn({
    name: 'player_id' ,
  })
  player!: Player;

  @Column({
    type: 'int',
    nullable: false,
  })
  character_id!: number;

  @Column({
    type: 'int',
    nullable: false,
  })
  level!: number;
}