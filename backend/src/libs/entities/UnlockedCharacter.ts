import {Column, Entity, JoinColumn, OneToOne, PrimaryGeneratedColumn} from 'typeorm';
import {Player} from './Player';
import {Character} from './Character';

@Entity()
export class UnlockedCharacter {

  @PrimaryGeneratedColumn()
  id!: number;

  @OneToOne(() => Player)
  @JoinColumn({ name: 'player_id' })
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